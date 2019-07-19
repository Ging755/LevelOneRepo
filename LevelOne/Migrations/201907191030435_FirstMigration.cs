namespace LevelOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RecipientName = c.String(),
                        RecipientSurname = c.String(),
                        RecipientAdress = c.String(),
                        RecipientZipCode = c.String(),
                        RecipientEmail = c.String(),
                        RecipientPhone = c.String(),
                        OrderReciveTime = c.String(),
                        OrderSent = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Orders");
        }
    }
}
