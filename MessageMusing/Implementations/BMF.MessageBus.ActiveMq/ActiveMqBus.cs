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
    public class ActiveMqBus : IMessageBus, IDisposable
    {
        private string _hostUrl = @"activemq:tcp://activemqhost:61616";

        internal ISerialiser _serialiser;
        internal IConnectionFactory _factory;
        internal IConnection _connection;
        internal ISession _session;
        internal Dictionary<string, IMessageProducer> _queues;

        private bool _disposed;

        public ActiveMqBus(ISerialiser serialiser)
        {
            _serialiser = serialiser;

            _factory = new NMSConnectionFactory(_hostUrl);
            _connection = _factory.CreateConnection();
            _session = _connection.CreateSession();
            _queues = new Dictionary<string, IMessageProducer>();
        }

        private void CreateQueue(string name)
        {
            var destination = SessionUtil.GetDestination(_session, "queue://" + name);
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

        public void Publish<T_message>(T_message message)
        {
            var body = _serialiser.Serialise(message);
            var msg = _session.CreateBytesMessage(body);

            _queues.FirstOrDefault().Value.Send(msg);
        }
            
    
        public void Subscribe<T_message>()
        {
            throw new NotImplementedException();
        }

        public int Subscribe<T_message>(Action<T_message> action)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<T_message>()
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<T_message>(int subscription)
        {
            throw new NotImplementedException();
        }
    }
}
