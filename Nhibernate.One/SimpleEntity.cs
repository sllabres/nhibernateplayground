using System;
using NUnit.Framework;
using FluentNHibernate.Mapping;
using System.Linq;

namespace Nhibernate.One
{
    public class SimpleEntity
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string OtherField { get; set; }
    }

    public class SimpleEntityMap : ClassMap<SimpleEntity>
    {
        public SimpleEntityMap()
        {
            Id(x => x.Id, "Id");
            Map(x => x.Name, "Name");
            Map(x => x.OtherField, "OtherField");
            Table("SimpleEntities");
        }
    }
}
