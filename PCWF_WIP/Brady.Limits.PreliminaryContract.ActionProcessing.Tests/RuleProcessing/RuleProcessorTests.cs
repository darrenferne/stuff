using Brady.Limits.PreliminaryContract.DataService.Concrete;
using Brady.Limits.PreliminaryContract.DataService.Core;
using Brady.Limits.PreliminaryContract.DataService.Interfaces;
using Brady.Limits.PreliminaryContract.Domain.Enums;
using Brady.Limits.PreliminaryContract.Domain.Models;
using Brady.Limits.PreliminaryContract.Level4File.Concrete;
using Brady.Limits.PreliminaryContract.Tests.Core;
using Brady.Limits.Rules.Domain;
using Brady.Limits.Rules.Processor.Client;
using BWF.DataServices.Nancy.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.DataService.Tests
{
    [TestClass]
    public class RuleProcessorTests : PreliminaryContractDataServiceTestContainer
    {
        private readonly Guid _systemReference = Guid.NewGuid();
        private readonly string _ruleProcessorUrl = "https://localhost:1234";
        private readonly string _clientFailureMsg = "Client call failed";

        private Mock<ILimitsRuleProcessorClient> GetClientMock(RegistrationAPIResponse expectedResponse = null, bool throwOnNullResponse = true)
        {
            var clientMock = new Mock<ILimitsRuleProcessorClient>();

            if (expectedResponse != null && throwOnNullResponse)
            {
                clientMock
                    .Setup(a => a.RegisterItem(It.Is<ItemRegistration>(i => i.SystemReference == _systemReference)))
                    .Returns(Task.FromResult(expectedResponse));
            }
            else
            {
                var setup = clientMock
                   .Setup(a => a.RegisterItem(It.IsAny<ItemRegistration>()));

                if (throwOnNullResponse)
                    setup.Throws(new Exception(_clientFailureMsg));
            }

            return clientMock;
        }

        private Mock<ILimitsRuleProcessorClientFactory> GetClientFactoryMock(Mock<ILimitsRuleProcessorClient> clientMock)
        {
            var factoryMock = new Mock<ILimitsRuleProcessorClientFactory>();
            factoryMock
                .Setup(m => m.CreateClient(It.IsAny<string>()))
                .Returns(clientMock.Object);

            return factoryMock;
        }

        public PreliminaryContractRuleProcessor GetRuleProcessor(string ruleProcessorUrl, RegistrationAPIResponse expectedResponse = null, bool throwOnNullResponse = true, bool isRegistered = true)
        {

            var dshsMock = new Mock<IDataServiceHostSettings>();

            var settingsMock = new Mock<IPreliminaryContractSettings>();
            settingsMock
                .SetupGet(s => s.RuleProcessorUrl)
                .Returns(ruleProcessorUrl);

            var systemRegistrationMock = new Mock<IPreliminaryContractSystemRegistration>();
            systemRegistrationMock
                .Setup(r => r.GetSystemReference())
                .Returns(isRegistered ? _systemReference : Guid.Empty);

            systemRegistrationMock
                .SetupGet(r => r.IsRegistered)
                .Returns(isRegistered);

            var clientMock = GetClientMock(expectedResponse, throwOnNullResponse);
            var clientFactoryMock = GetClientFactoryMock(clientMock);
            
            var ruleProcessor = new PreliminaryContractRuleProcessor(_logger.Object, authorisation, dshsMock.Object, settingsMock.Object, PreliminaryContractRepository, PreliminaryContractDataService, systemRegistrationMock.Object, clientFactoryMock.Object);

            return ruleProcessor;
        }

        [TestInitialize]
        public void Initialize()
        {
            //Persist a file so the file id and header id are not the same
            var fileFailedParse = FluentContractFile
                .Create()
                .ApplyDefaults()
                .Save(this);
        }

        [TestMethod]
        public void Registering_a_valid_contract_should_succeed()
        {
            var contract = FluentContract
                .Create()
                .ApplyDefaults()
                .Save(this);

            var expectedResponse = new RegistrationAPIResponse(Guid.NewGuid());

            var ruleProcessor = GetRuleProcessor(_ruleProcessorUrl, expectedResponse);

            var response = ruleProcessor.RegisterContractItem(contract);

            Assert.AreEqual(RegistrationAPIState.Registered, response.State);
            Assert.AreEqual(expectedResponse.RegistrationReference, response.RegistrationReference);

            var trackingReferences = PreliminaryContractRepository.GetAll<ContractTrackingReference>().ToList();
            Assert.AreEqual(1, trackingReferences.Count);
            Assert.AreEqual(expectedResponse.RegistrationReference.ToString(), trackingReferences[0].ItemReference);
            Assert.AreEqual(ContractTrackingState.EvaluationPending, trackingReferences[0].State);
            Assert.AreNotEqual(0, trackingReferences[0].Id);
            Assert.AreNotEqual(default(DateTime), trackingReferences[0].CreatedAt);
            Assert.AreNotEqual(default(DateTime), trackingReferences[0].UpdatedAt);
        }

        [TestMethod]
        public void Registering_a_contract_when_the_contract_is_null_should_fail()
        {
            var ruleProcessor = GetRuleProcessor(_ruleProcessorUrl);

            var response = ruleProcessor.RegisterContractItem(null);

            Assert.AreEqual(RegistrationAPIState.Failed, response.State);
            Assert.AreEqual("Failed to register contract item: Contract not supplied", response.Messages[0]);
        }

        [TestMethod]
        public void Creating_an_instance_of_the_rule_processor_when_the_rule_processor_url_is_not_configured_should_fail()
        {
            try
            {
                var ruleProcessor = GetRuleProcessor("");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Failed to start the contract rule processor. The Rule Processor Url is not defined", ex.Message);
            }
        }

        [TestMethod]
        public void Registering_a_contract_when_the_system_is_not_registered_should_fail()
        {
            var contract = FluentContract
                .Create()
                .ApplyDefaults()
                .Save(this);

            var ruleProcessor = GetRuleProcessor(_ruleProcessorUrl, null, true, false);

            var response = ruleProcessor.RegisterContractItem(contract);

            Assert.AreEqual(RegistrationAPIState.Failed, response.State);
            Assert.AreEqual("Failed to register contract item: System is not registered with the rules engine", response.Messages[0]);
        }

        [TestMethod]
        public void Registering_a_contract_when_the_client_fails_should_fail()
        {
            var contract = FluentContract
                .Create()
                .ApplyDefaults()
                .Save(this);

            var ruleProcessor = GetRuleProcessor(_ruleProcessorUrl);

            var response = ruleProcessor.RegisterContractItem(contract);

            Assert.AreEqual(RegistrationAPIState.Failed, response.State);
            Assert.AreEqual($"Failed to register contract item: {_clientFailureMsg}", response.Messages[0]);
        }

        [TestMethod]
        public void Registering_a_contract_when_the_client_returns_null_should_fail()
        {
            var contract = FluentContract
                .Create()
                .ApplyDefaults()
                .Save(this);

            var ruleProcessor = GetRuleProcessor(_ruleProcessorUrl, null, false);

            var response = ruleProcessor.RegisterContractItem(contract);

            Assert.AreEqual(RegistrationAPIState.Failed, response.State);
            Assert.AreEqual("Failed to register contract item: Communication with the rules engine failed", response.Messages[0]);
        }

        [TestMethod]
        public void Registering_a_contract_that_is_on_hold_should_fail()
        {
            var contract = FluentContract
                .Create()
                .ApplyDefaults()
                .OnHold(true)
                .Save(this);

            var ruleProcessor = GetRuleProcessor(_ruleProcessorUrl, null, false);

            var response = ruleProcessor.RegisterContractItem(contract);

            Assert.AreEqual(RegistrationAPIState.Failed, response.State);
            Assert.AreEqual("Failed to register contract items: The file is flagged as 'Hold From Approval'", response.Messages[0]);
        }
    }
}
