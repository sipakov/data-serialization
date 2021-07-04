using System.Threading.Tasks;

namespace data_serialization.Interfaceses
{
    public interface IXmlToJsonConverter
    {
        Task<bool> Convert(string pathToTargetXml, string pathToOutputJson);
    }
}