using Apache.NMS;
using Apache.NMS.Util;
using BMF.MessageBus.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.ActiveMq
{
    public class AMQMessageBus : IMessageBus, IDisposable
    {
        private string _hostUrl = @"activemq:tcp://activemqhost:61616";

        internal IMessageBusSerialiser _serialiser;
        internal IConnectionFactory _factory;
        internal IConnection _connection;
        internal ISession _session;
        internal Dictionary<string, IMessageProducer> _queues;

        private bool _disposed;

        public AMQMessageBus(IMessageBusSerialiser serialiser)
        {
            _serialiser = serialiser;

            _factory = new NMSConnectionFactory(_hostUrl);
            _connection = _factory.CreateConnection();
            _session = _connection.CreateSession();
            _queues = new Dictionary<string, IMessageProducer>();
        }

        private void CreateQueue(string name, bool forTopic = false)
        {
            // Examples for getting a destination:
            //
            // Hard coded destinations:
            //    IDestination destination = session.GetQueue("FOO.BAR");
            //    Debug.Assert(destination is IQueue);
            //    IDestination destination = session.GetTopic("FOO.BAR");
            //    Debug.Assert(destination is ITopic);
            //
            // Embedded destination type in the name:
            //    IDestination destination = SessionUtil.GetDestination(session, "queue://FOO.BAR");
            //    Debug.Assert(destination is IQueue);
            //    IDestination destination = SessionUtil.GetDestination(session, "topic://FOO.BAR");
            //    Debug.Assert(destination is ITopic);
            //
            // Defaults to queue if type is not specified:
            //    IDestination destination = SessionUtil.GetDestination(session, "FOO.BAR");
            //    Debug.Assert(destination is IQueue);
            //
            // .NET 3.5 Supports Extension methods for a simplified syntax:
            //    IDestination destination = session.GetDestination("queue://FOO.BAR");
            //    Debug.Assert(destination is IQueue);
            //    IDestination destination = session.GetDestination("topic://FOO.BAR");
            //    Debug.Assert(destination is ITopic);

            var destinationType = forTopic ? "topic://" : "queue://";
            var destination = _session.GetDestination(destinationType + name);
                        
            var producer = _session.CreateProducer(destination);
            producer.DeliveryMode = MsgDeliveryMode.Persistent;
            
            _queues.Add(name, producer);
        }
        
        private void Dispose(bool destruct)
        {
            if (!_disposed)
            {
                if (_session != null)
                {
                    _session.Dispose();
                    _session = null;
                }

                if (_connection != null)
                {
                    _connection.Dispose();
                    _connection = null;
                }

                if (_factory != null)
                    _factory = null;

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Publish<T_message>(T_message message)
        {
            var body = _serialiser.Serialise(message);
            var msg = _session.CreateBytesMessage(body);

            _queues.FirstOrDefault().Value.Send(msg);
        }

        public void Subscribe(Type messageType)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<T_message>()
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(Type messageType)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<T_message>()
        {
            throw new NotImplementedException();
        }
    }
}
