/*9fbef606107a605d69c0edbcd8029e5d*/
#region
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Publishing;
using Sitecore.SecurityModel;
using Sitecore.Workflows.Simple;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
#endregion

namespace FWD.Foundation.Multisite.Infrastructure.Events
{
    [ExcludeFromCodeCoverage]
    public class DeleteApproved
    {
        public void Process(WorkflowPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Item currentItem = args.DataItem;
            try
            {
                if (currentItem != null)
                {
                    Item innerItem = currentItem;
                    Item parent = innerItem.Parent;
                    innerItem.Recycle(); //send to recyclebin from master db

                    foreach (Database database in GetTargets(parent))
                    {
                        //publish the parrent for each database
                        var options = new PublishOptions(parent.Database, database, PublishMode.SingleItem, parent.Language, DateTime.Now)
                        { RootItem = parent, Deep = true };
                        new Publisher(options).PublishAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, this);
            }
        }
        private IEnumerable<Database> GetTargets(Item item)
        {
            using (new SecurityDisabler())
            {
                Item targets = item.Database.Items["/sitecore/system/publishing targets"];
                if (targets != null)
                {
                    var list = new ArrayList();
                    foreach (Item target in targets.Children)
                    {
                        string name = target["Target database"];
                        if (name.Length > 0)
                        {
                            Database database = Factory.GetDatabase(name, false);
                            if (database != null)
                            {
                                list.Add(database);
                            }
                            else
                            {
                                Log.Warn("Unknown database in AutoPublisherHandler: " + name, this);
                            }
                        }
                    }
                    return (list.ToArray(typeof(Database)) as Database[]);
                }
            }
            return new Database[0];
        }
    }
}