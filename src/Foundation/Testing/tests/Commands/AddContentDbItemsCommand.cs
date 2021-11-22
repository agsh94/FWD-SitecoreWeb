/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ploeh.AutoFixture.Kernel;
using Sitecore.FakeDb;

#endregion

namespace FWD.Foundation.Testing.Commands
{
    [ExcludeFromCodeCoverage]
    public class AddContentDbItemsCommand : GenericCommand<DbItem[]>
  {
      protected override void ExecuteAction(DbItem[] specimen, ISpecimenContext context)
      {
          var db = (Db)context?.Resolve(typeof(Db));
          if (db != null) specimen.ToList().ForEach(db.Add);
      }
  }
}