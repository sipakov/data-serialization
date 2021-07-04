using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace data_serialization.Interfaceses
{
    public interface IXmlToJsonConverter
    {
        Task<bool> Convert([NotNull]string pathToTargetXml, [NotNull]string pathToOutputJson);
    }
}