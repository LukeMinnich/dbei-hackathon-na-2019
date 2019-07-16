using System;
using System.Collections.Generic;
using System.Linq;
using Kataclysm.Common;
using Kataclysm.Common.Extensions;
using MathNet.Spatial.Euclidean;
using Newtonsoft.Json;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class NodalResponseCollection : ResponseCollection<NodalResponse>
    {
        public Point2D Coordinate { get; }

        [JsonConstructor]
        public NodalResponseCollection(IEnumerable<NodalResponse> objects)
            : base(objects)
        {
            Coordinate = objects.First().Coordinate;
        }
        
        public NodalResponseCollection(string elementId, Point2D coordinate)
            : base(elementId)
        {
            Coordinate = coordinate;
        }

        public override NodalResponse SuperimposeResponsesAtLoadCase(LoadCase loadCase)
        {
            NodalResponse response = base.SuperimposeResponsesAtLoadCase(loadCase);

            response.Coordinate = Coordinate;

            return response;
        }

        public override NodalResponse EnvelopeResponsesAbsolute(IEnumerable<LoadCase> loadCases)
        {
            IEnumerable<NodalResponse> loadCaseResponses = GetSuperimposedResponsesAtLoadCases(loadCases);

            var envelope = new NodalResponse(ElementId, LoadPattern.Mixed, Coordinate)
            {
                ExternalForce = new NodalForce {LoadPattern = LoadPattern.Mixed},
                Reaction = new NodalForce {LoadPattern = LoadPattern.Mixed},
                Displacement = new NodalDisplacement {LoadPattern = LoadPattern.Mixed},
                Stress = new NodalStress { LoadPattern = LoadPattern.Mixed}
            };

            foreach (var response in loadCaseResponses)
            {
                envelope.ExternalForce.Fx = envelope.ExternalForce.Fx.AbsoluteValueEnvelope(response.ExternalForce.Fx);
                envelope.ExternalForce.Fy = envelope.ExternalForce.Fy.AbsoluteValueEnvelope(response.ExternalForce.Fy);
                envelope.ExternalForce.Mz = envelope.ExternalForce.Mz.AbsoluteValueEnvelope(response.ExternalForce.Mz);
                
                envelope.Reaction.Fx = envelope.Reaction.Fx.AbsoluteValueEnvelope(response.Reaction.Fx);
                envelope.Reaction.Fy = envelope.Reaction.Fy.AbsoluteValueEnvelope(response.Reaction.Fy);
                envelope.Reaction.Mz = envelope.Reaction.Mz.AbsoluteValueEnvelope(response.Reaction.Mz);

                envelope.Displacement.Ux = envelope.Displacement.Ux.AbsoluteValueEnvelope(response.Displacement.Ux);
                envelope.Displacement.Uy = envelope.Displacement.Uy.AbsoluteValueEnvelope(response.Displacement.Uy);
                envelope.Displacement.Rz = envelope.Displacement.Rz.AbsoluteValueEnvelope(response.Displacement.Rz);

                envelope.Stress.Sigma_x = envelope.Stress.Sigma_x.AbsoluteValueEnvelope(response.Stress.Sigma_x);
                envelope.Stress.Sigma_y = envelope.Stress.Sigma_y.AbsoluteValueEnvelope(response.Stress.Sigma_y);
                envelope.Stress.Sigma_xy = envelope.Stress.Sigma_xy.AbsoluteValueEnvelope(response.Stress.Sigma_xy);
            }

            return envelope;
        }

        public override Tuple<NodalResponse, NodalResponse> EnvelopeResponsesMinMax(IEnumerable<LoadCase> loadCases)
        {
            throw new NotImplementedException();
        }
    }
}