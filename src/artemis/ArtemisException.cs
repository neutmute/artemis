using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artemis
{
    /// <summary>
    /// Doesn't do anything special apart from simplified syntax in order to use string.format
    /// (see static methods)
    /// </summary>
    /// <remarks>
    /// The pure static throw was not being handled by exception management properly
    /// change to the thread static singleton fixed this
    /// </remarks>
    [Serializable]
    public class ArtemisException : Exception
    {
        #region Constructors
        /// <summary>
        /// Use the static constructors
        /// </summary>
        protected ArtemisException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Use the static constructors
        /// </summary>
        protected ArtemisException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Create a new Exception
        /// </summary>
        /// <remarks>
        /// Only use this exception if no suitable alternative exists - eg: use InvalidArgumentException if it is a better fit
        /// </remarks>
        public static ArtemisException Create(string format, params object[] args)
        {
            string message = string.Format(format, args);
            ArtemisException exception = new ArtemisException(message);
            return exception;
        }

        /// <summary>
        /// Create a new Exception
        /// </summary>
        /// <remarks>
        /// Only use this exception if no suitable alternative exists - eg: use InvalidArgumentException if it is a better fit
        /// </remarks>
        public static ArtemisException Create(Exception innerException, string format, params object[] args)
        {
            string message = string.Format(format, args);
            ArtemisException exception = new ArtemisException(message, innerException);
            return exception;
        }
        #endregion
    }
}
