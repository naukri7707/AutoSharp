using System.Collections.Generic;

namespace AutoSharp
{
    public static class Notify
    {
        private static readonly Queue<Message> messageQuene = new Queue<Message>();

        /// <summary>
        /// Send <paramref name="message"/> to all modules
        /// </summary>
        /// <param name="message">The message to send.</param>
        public static void Broadcast(string message)
        {
            EnqueueMessage(MessageType.Broadcast, null, message);
        }

        /// <summary>
        /// Send <paramref name="message"/> to all modules with <paramref name="targetName"/>.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="targetName">The name of target.</param>
        /// <param name="strength"></param>
        public static void Multicast(string message, string targetName)
        {
            EnqueueMessage(MessageType.Multicast, message, targetName);
        }

        /// <summary>
        /// Send message to first target module.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="message"></param>
        public static void Unicast(string message, string targetName)
        {
            EnqueueMessage(MessageType.Unicast, message, targetName);
        }

        internal static bool TryDequeueMessage(out Message message)
        {
            if (messageQuene.Count > 0)
            {
                message = messageQuene.Dequeue();
                return true;
            }
            message = default;
            return false;
        }

        private static void EnqueueMessage(MessageType messageType, string targetName, string message)
        {
            var msg = new Message(messageType, targetName, message);
            messageQuene.Enqueue(msg);
        }

        internal enum MessageType
        {
            Unicast,

            Multicast,

            Broadcast
        }

        internal readonly struct Message
        {
            public Message(MessageType type, string target, string message)
            {
                this.type = type;
                this.target = target;
                this.message = message;
            }

            internal readonly string message;

            internal readonly string target;

            internal readonly MessageType type;
        }
    }
}
