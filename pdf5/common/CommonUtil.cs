using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Pdf;
using static Aspose.Pdf.UnifiedSaveOptions;

namespace pdf5.common
{
    public class CommonUtil
    {
        public void ShowProgressOnConsole(DocSaveOptions.ProgressEventHandlerInfo eventInfo)
        {


            Console.WriteLine("进入到showprogress");
            switch (eventInfo.EventType)
            {


                case ProgressEventType.TotalProgress:
                    Debug.WriteLine(string.Format("{0}  - Conversion progress : {1}% .", DateTime.Now.ToLongTimeString(), eventInfo.Value.ToString()));
                    break;

                case ProgressEventType.SourcePageAnalized:
                    Debug.WriteLine(string.Format("{0}  - Source page {1} of {2} analyzed.", DateTime.Now.ToLongTimeString(), eventInfo.Value.ToString(), eventInfo.MaxValue.ToString()));
                    break;
                case ProgressEventType.ResultPageCreated:
                    Debug.WriteLine(string.Format("{0}  - Result page's {1} of {2} layout created.", DateTime.Now.ToLongTimeString(), eventInfo.Value.ToString(), eventInfo.MaxValue.ToString()));
                    break;
                case ProgressEventType.ResultPageSaved:
                    Debug.WriteLine(string.Format("{0}  - Result page {1} of {2} exported.", DateTime.Now.ToLongTimeString(), eventInfo.Value.ToString(), eventInfo.MaxValue.ToString()));
                    //插入
                    break;

                default:
                    break;
            }
        }
    }
}
