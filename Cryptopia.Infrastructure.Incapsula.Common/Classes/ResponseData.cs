using Cryptopia.Infrastructure.Incapsula.Common.Enums;
using Cryptopia.Infrastructure.Incapsula.Common.Responses;
using System.Runtime.Serialization;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  [DataContract]
  public class ResponseData
  {
    public ResponseData(ResponseBase response)
    {
      if (response == null)
      {
        ResponseCode = ResponseCode.UnknownResponse;
        ResponseMessage = "No response object available to determine result";
      }
      else
      {
        ResponseCode = (ResponseCode)response.res;
        ResponseMessage = response.res_message;
      }
    }

    [DataMember]
    public string ResponseMessage { get; private set; }

    [DataMember]
    public ResponseCode ResponseCode { get; private set; }
  }
}
