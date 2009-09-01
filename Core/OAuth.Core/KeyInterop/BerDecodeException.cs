using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace OAuth.Core.KeyInterop
{
    [Serializable]
    public sealed class BerDecodeException : Exception
    {
        readonly int _position;

        public BerDecodeException()
        {
        }

        public BerDecodeException(String message)
            : base(message)
        {
        }

        public BerDecodeException(String message, Exception ex)
            : base(message, ex)
        {
        }

        public BerDecodeException(String message, int position)
            : base(message)
        {
            _position = position;
        }

        public BerDecodeException(String message, int position, Exception ex)
            : base(message, ex)
        {
            _position = position;
        }

        BerDecodeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _position = info.GetInt32("Position");
        }

        public int Position
        {
            get { return _position; }
        }

        public override string Message
        {
            get
            {
                var sb = new StringBuilder(base.Message);

                sb.AppendFormat(" (Position {0}){1}",
                                _position, Environment.NewLine);

                return sb.ToString();
            }
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Position", _position);
        }
    }
}