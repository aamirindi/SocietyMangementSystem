using SocietyMVC.Models;

public class VisitorViewModel
{
    public VisitorsModel NewVisitor { get; set; }
    public IEnumerable<VisitorsModel> ApprovedVisitors { get; set; } = new List<VisitorsModel>();
}