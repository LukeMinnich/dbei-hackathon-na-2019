using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Kataclysm.Common;
using Kataclysm.Common.Extensions;
using Kataclysm.Common.Reporting;
using Katerra.Apollo.Structures.Common.Units;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public static class RigidAnalysisTabularReport
    {
        public static Table GenerateShearWallForceStiffnessTable(RigidAnalysis analysis)
        {
            var table = new Table("Rigid Analysis Wall Force Determination");
            
            table.AddColumn("level", ColumnDataType.Text);
            table.AddColumn("load_category", ColumnDataType.Text);
            table.AddColumn("load_case", ColumnDataType.Text);
            table.AddColumn("wall_panel", ColumnDataType.Text);
            table.AddColumn("R", ColumnDataType.Number, "R (k/in)", 1);
            table.AddColumn("d", ColumnDataType.Number, "d (in)", 1);
            table.AddColumn("Rd", ColumnDataType.Number, "$R d$ (k-ft/in)", 0);
            table.AddColumn("Rd2", ColumnDataType.Number, "$R d^2$ (k-ft^2/in)", 0);
            table.AddColumn("direct_force", ColumnDataType.Number, "Direct Force $F_v$ (lb)", 0);
            table.AddColumn("torsional_force", ColumnDataType.Number, "Torsional Force $F_t$ (lb)", 0);
            table.AddColumn("total_force", ColumnDataType.Number, "Total Force $F_v + F_t$ (lb)", 0);
            
            table.SetExpression("Rd", "R * d");
            table.SetExpression("Rd2", "R * d * d");
            table.SetExpression("total_force", "direct_force + torsional_force");

            table.SortByColumns("load_case", SortOrder.Ascending, "level", SortOrder.Descending, "wall_panel",
                SortOrder.Ascending);

            foreach (AnalyticalWallLateral wall in analysis.Walls)
            {
                ShearWallPanelRigidAnalysisParameters stiffnessAtWall = analysis.WallStiffnesses[wall.UniqueId];
                ShearWallPanelResponseCollection resultsAtWall = analysis.Responses.WallLoadCaseResults[wall.UniqueId];

                foreach (ShearWallPanelResponse response in resultsAtWall)
                {
                    DataRow row = table.NewRow();

                    row["level"] = wall.TopLevel.Name;
                    row["load_category"] = response.LoadCase.LoadPatternType;
                    row["load_case"] = response.LoadCase;
                    row["wall_panel"] = wall.UniqueId;
                    row["R"] = stiffnessAtWall.K.ConvertTo(ForcePerLengthUnit.KipPerInch);
                    row["d"] = stiffnessAtWall.SignedOffsetFromCenterOfRigidity.ConvertTo(LengthUnit.Foot);
                    row["direct_force"] = response.DirectShear.ConvertTo(ForceUnit.Pound);
                    row["torsional_force"] = response.TorsionalShear.ConvertTo(ForceUnit.Pound);
                    
                    table.AddRow(row);
                }
            }

            return table;
        }
        
        public static Table GenerateTorsionalIrregularityTable(IEnumerable<PointDrift> drifts)
        {
            var table = new Table("Seismic Torsional Irregularity Check");
            
            table.AddColumn("level", ColumnDataType.Text);
            table.AddColumn("load category", ColumnDataType.Text);
            table.AddColumn("load_case", ColumnDataType.Text);
            table.AddColumn("drift_direction", ColumnDataType.Text);
            table.AddColumn("node_a", ColumnDataType.Text);
            table.AddColumn("node_a_drift", ColumnDataType.Number,"Node A Drift (%)", 3);
            table.AddColumn("node_b", ColumnDataType.Text);
            table.AddColumn("node_b_drift", ColumnDataType.Number, "Node B Drift (%)", 3);
            table.AddColumn("delta_avg", ColumnDataType.Number, @"$\delta_{avg}$ (%)", 3);
            table.AddColumn("1.2_delta_avg", ColumnDataType.Number, @"$1.2 \delta_{avg}$ (%)", 3);
            table.AddColumn("1.4_delta_avg", ColumnDataType.Number, @"$1.4 \delta_{avg}$ (%)", 3);
            table.AddColumn("delta_max", ColumnDataType.Number, @"$\delta_{max}$ (%)", 3);
            table.AddColumn("ratio", ColumnDataType.Number, @"$\delta_{max} / \delta_{avg}$", 3);
            table.AddColumn("status", ColumnDataType.Text);
            
            table.SetColumnTextCapitalization("load category", StringCase.AllCaps);

            table.SetExpression("delta_avg", "(node_a_drift + node_b_drift) / 2");
            table.SetExpression("1.2_delta_avg", "1.2 * delta_avg");
            table.SetExpression("1.4_delta_avg", "1.4 * delta_avg");
            table.SetExpression("delta_max", "IIF(node_a_drift > node_b_drift, node_a_drift, node_b_drift)");
            table.SetExpression("ratio", "delta_max / delta_avg");
            table.SetExpression("status", $"IIF(delta_max <= [1.2_delta_avg], 'OK', IIF(delta_max <= [1.4_delta_avg]," +
                                          $"'{TorsionalIrregularityStatus.Type1A.GetDescription()}'," +
                                          $"'{TorsionalIrregularityStatus.Type1B.GetDescription()}'))");
            
            table.SortByColumn("level", SortOrder.Descending);
            
            // Comparisons shall only be made within the relevant group
            var groupedDrifts = drifts.Where(d => d.LoadCategory == LoadPatternType.Earthquake)
                                      .GroupBy(d => new {d.Level, d.LoadCase, d.Direction});
            
            foreach (var driftGroup in groupedDrifts)
            {
                List<PointDrift> driftsInGroup = driftGroup.ToList();
                
                for (var i = 0; i < driftsInGroup.Count; i++)
                {
                    var driftIrrelevant = false;
                    
                    for (var j = 0; j < driftsInGroup.Count; j++)
                    {
                        // Avoid repeating comparisons and comparing to oneself
                        if (j <= i) continue;

                        var pointA = driftsInGroup[i];
                        var pointB = driftsInGroup[j];

                        if (!PredominantLoadDirectionCoincidesWithDriftDirection(pointA))
                        {
                            driftIrrelevant = true;
                            break;
                        }

                        DataRow row = table.NewRow();

                        row["level"] = pointA.Level;
                        row["load category"] = pointA.LoadCategory;
                        row["load_case"] = pointA.LoadCase;
                        row["drift_direction"] = pointA.Direction;
    
                        row["node_a"] = pointA.Name;
                        row["node_b"] = pointB.Name;

                        row["node_a_drift"] = pointA.Drift.Value * 100;
                        row["node_b_drift"] = pointB.Drift.Value * 100;

                        table.AddRow(row);
                    }

                    if (driftIrrelevant) break;
                }
            }

            return table;
        }

        public static bool PredominantLoadDirectionCoincidesWithDriftDirection(PointDrift drift)
        {
            switch (drift.Direction)
            {
                case LateralDirection.X:
                    switch (drift.LoadCase.PredominantDirection)
                    {
                        case PredominantDirection.X:
                        case PredominantDirection.Both:
                            return true;
                        case PredominantDirection.Y:
                            return false;
                        default:
                            throw new InvalidEnumArgumentException();
                    }
                case LateralDirection.Y:
                    switch (drift.LoadCase.PredominantDirection)
                    {
                        case PredominantDirection.X:
                            return false;
                        case PredominantDirection.Y:
                        case PredominantDirection.Both:
                            return true;
                        default:
                            throw new InvalidEnumArgumentException();
                    }
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}