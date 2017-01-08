using System.Runtime.Serialization;

namespace GitToVsts.Model
{
    /// <summary>
    ///     HelperObject for returning responses.
    /// </summary>
    [DataContract]
    public class Response<T>
    {
        /// <summary>
        ///     DataMember that contains the return value of type "T" of a class200.
        /// </summary>
        [DataMember]
        public T Value { get; set; }

        /// <summary>
        ///     DataMember that contains a responsecode.
        /// </summary>
        [DataMember]
        public int Code { get; set; }
    }
}