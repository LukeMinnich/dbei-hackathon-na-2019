using System;
using System.Collections.Generic;
using System.Linq;
using Kataclysm.Common;
using Kataclysm.Common.Units.Conversion;
using MathNet.Spatial.Euclidean;
using Newtonsoft.Json;

namespace Kataclysm.StructuralAnalysis.Rigid

{
    public class ShearWallPanelResponseCollection : ResponseCollection<ShearWallPanelResponse>
    {
        public LineSegment2D WallLocation { get; }
        
        [JsonConstructor]
        public ShearWallPanelResponseCollection(IEnumerable<ShearWallPanelResponse> objects)
            : base(objects)
        {
            WallLocation = objects.First().WallLocation;
        }

        public ShearWallPanelResponseCollection(string elementId, LineSegment2D wallLocation)
            : base(elementId)
        {
            WallLocation = wallLocation;
        }

        public override ShearWallPanelResponse SuperimposeResponsesAtLoadCase(LoadCase loadCase)
        {
            ShearWallPanelResponse response = base.SuperimposeResponsesAtLoadCase(loadCase);

            response.WallLocation = WallLocation;

            return response;
        }

        public override ShearWallPanelResponse EnvelopeResponsesAbsolute(IEnumerable<LoadCase> loadCases)
        {
            IEnumerable<ShearWallPanelResponse> loadCaseResponses = GetSuperimposedResponsesAtLoadCases(loadCases);
            
            var envelope = new ShearWallPanelResponse(ElementId, LoadPattern.Mixed, WallLocation);

            foreach (var response in loadCaseResponses)
            {
                envelope.DirectShear = (Force) Result.AbsoluteValueEnvelope(envelope.DirectShear, response.DirectShear);
                envelope.TorsionalShear = (Force) Result.AbsoluteValueEnvelope(envelope.TorsionalShear, response.TorsionalShear);
            }

            return envelope;
        }

        public override Tuple<ShearWallPanelResponse, ShearWallPanelResponse> EnvelopeResponsesMinMax(IEnumerable<LoadCase> loadCases)
        {
            IEnumerable<ShearWallPanelResponse> loadCaseResponses = GetSuperimposedResponsesAtLoadCases(loadCases);

            var minEnvelope = new ShearWallPanelResponse(ElementId, LoadPattern.Mixed, WallLocation);
            var maxEnvelope = new ShearWallPanelResponse(ElementId, LoadPattern.Mixed, WallLocation);

            foreach (var response in loadCaseResponses)
            {
                minEnvelope.DirectShear = (Force) Result.MinValueEnvelope(minEnvelope.DirectShear, response.DirectShear);
                maxEnvelope.DirectShear = (Force) Result.MaxValueEnvelope(maxEnvelope.DirectShear, response.DirectShear);
                
                minEnvelope.TorsionalShear = (Force) Result.MinValueEnvelope(minEnvelope.TorsionalShear, response.TorsionalShear);
                maxEnvelope.TorsionalShear = (Force) Result.MaxValueEnvelope(maxEnvelope.TorsionalShear, response.TorsionalShear);
            }
            
            return new Tuple<ShearWallPanelResponse, ShearWallPanelResponse>(minEnvelope, maxEnvelope);
        }
    }
}