using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;

namespace GetAllObjectArea1
{
    public class Class1
    {
        //测试输出hello world
        [CommandMethod("HelloWorld")]
        public void HelloWorld()
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("Hello World");

            Clipboard.SetText("Hello World");
        }
    }
    public class AreaCalculator
    {
        [CommandMethod("aa")]
        public void CalculateArea()
        {
            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor acEd = acDoc.Editor;

            // 获取选定对象  
            PromptSelectionResult acSelRes = acEd.GetSelection();
            if (acSelRes.Status != PromptStatus.OK)
                return;

            SelectionSet acSelSet = acSelRes.Value;

            double totalArea = 0.0;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                foreach (SelectedObject acSelObj in acSelSet)
                {
                    // 假设只处理Polyline  
                    if (acSelObj.ObjectId.ObjectClass.IsDerivedFrom(RXClass.GetClass(typeof(Polyline))))
                    {
                        Polyline pline = acTrans.GetObject(acSelObj.ObjectId, OpenMode.ForRead) as Polyline;
                        if (pline.Closed)
                        {
                            // 使用Polyline的Area属性（注意：这可能需要额外的逻辑来确保Polyline是闭合的）  
                            totalArea += pline.Area;
                        }
                        else
                        {
                            acEd.WriteMessage($"曲线未闭合");
                        }
                    }
                }

                acTrans.Commit();
            }

            acEd.WriteMessage($"选中对象的面积: {totalArea:F2} 平方米\n");
            
            Clipboard.SetText(totalArea.ToString());
        }
    }
}