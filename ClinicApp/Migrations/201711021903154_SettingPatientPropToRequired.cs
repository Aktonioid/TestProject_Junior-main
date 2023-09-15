namespace ClinicApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SettingPatientPropToRequired : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Requests", "Patient_Id", "dbo.PatientCards");
            DropIndex("dbo.Requests", new[] { "Patient_Id" });
            AlterColumn("dbo.Requests", "Patient_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Requests", "Patient_Id");
            AddForeignKey("dbo.Requests", "Patient_Id", "dbo.PatientCards", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Requests", "Patient_Id", "dbo.PatientCards");
            DropIndex("dbo.Requests", new[] { "Patient_Id" });
            AlterColumn("dbo.Requests", "Patient_Id", c => c.Int());
            CreateIndex("dbo.Requests", "Patient_Id");
            AddForeignKey("dbo.Requests", "Patient_Id", "dbo.PatientCards", "Id");
        }
    }
}
