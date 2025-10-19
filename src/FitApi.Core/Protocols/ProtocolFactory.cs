using FitApi.Core.Domain.Assessments.Models;
using FitApi.Core.Protocols.JacksonPollock;

namespace FitApi.Core.Protocols;

public static class ProtocolFactory
{
    public static AbstractProtocol CreateProtocol(BodyAssessment assessment, AssessmentsProtocols protocol)
    {
        return protocol switch
        {
            AssessmentsProtocols.Jp7 => new JacksonPollock7Folds(assessment),
            AssessmentsProtocols.Jp3 => new JacksonPollock3Folds(assessment),
            _ => throw new ArgumentOutOfRangeException(nameof(protocol), protocol, null)
        };
    }
}