using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;

namespace Subtle
{
    class Program
    {
        static void Main(string[] args)
        {
            var srtFileName = "C:\\Users\\robko\\Desktop\\1_2.srt";
            var subtitles = FillSubtitles(srtFileName);
            var time = new TimeSpan();
            // Timer timer = new Timer(ShowSubtitles, null, 0, 100);
            // while (_currentTime <= new TimeSpan(0, 0, 0, 30))
            // {

            // }
            var startListTime = subtitles.First().StartTime;
            var endListTime = subtitles.Last().EndTime;

            if (subtitles.Count == 0)
            {
                Console.WriteLine("No subtitles produced");
                return;
            }

            var priorEndTime = TimeSpan.MinValue;
            while (startListTime < endListTime)
            {
                var subtitle = subtitles.Dequeue();
                
                //Console.WriteLine("--");
                Console.Clear();
                if (priorEndTime != TimeSpan.MinValue)
                    Thread.Sleep(subtitle.StartTime-priorEndTime);

                priorEndTime = subtitle.EndTime;
                Console.WriteLine(subtitle.Text);
                Thread.Sleep(subtitle.EndTime - subtitle.StartTime);
                
                // wait while within timespan
                // Sleep(this endTime - this startTime)
                // wait while in between entries
                // Sleep(next startTime - prev endtime)
                //break;
            }
            // for (var ts = startListTime; ts <= endListTime; ts = ts.AddMinutes(interval))
            // {
            //     times.Add(ts.TimeOfDay);
            // }

            // Suspend the screen.
            Console.ReadLine();
            Console.WriteLine("complete");
            Console.WriteLine("End");
        }

        static Queue<SubtitleViewModel> FillSubtitles(string srtFileName)
        {

            int counter = 0;
            string line;
            int PartCollected = 0;

            var list = new Queue<SubtitleViewModel>();
            // Read the file and display it line by line.
            using (var file = File.OpenText(srtFileName))
            {
                //Console.WriteLine("writing to list");
                SubtitleViewModel svm = new SubtitleViewModel();
                while ((line = file.ReadLine()) != null)
                {
                    //Console.WriteLine(line);
                    int intParsed;
                    if (PartCollected == 0)
                    {
                        if (int.TryParse(line.Trim(), out intParsed))
                        {
                            svm.Id = intParsed;
                            PartCollected = 1;
                            continue;
                        }
                    }
                    if (PartCollected == 1)
                    {
                        if (line.Contains("-->"))
                        {
                            string[] times = line.Split(new string[] { " --> " }, StringSplitOptions.RemoveEmptyEntries);
                            svm.StartTime = TimeSpan.Parse(times[0].Replace(",", "."));
                            svm.EndTime = TimeSpan.Parse(times[1].Replace(",", "."));
                            //Console.WriteLine("{0} to {1}", svm.StartTime, svm.EndTime);
                            PartCollected = 2;
                            continue;
                        }
                    }
                    if (PartCollected == 2)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            if (!string.IsNullOrEmpty(svm.Text))
                                svm.Text = svm.Text + System.Environment.NewLine;
                            svm.Text = svm.Text + line;
                            //Console.WriteLine("dialogue:" + line);
                            continue;
                        }
                    }
                    if (line == string.Empty)
                    {
                        // reset
                        PartCollected = 0;
                        list.Enqueue(svm);
                        svm = new SubtitleViewModel();
                        continue;
                    }
                    counter++;
                }
            }
            // Console.WriteLine("now Let's see what's in list");
            // foreach (var item in list)
            //     Console.WriteLine("{0}/{1}/{2}/{3}", item.Id, item.StartTime, item.EndTime, item.Text);
            return list;
        }
    }
    public class SubtitleViewModel
    {
        public int Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Text { get; set; }
    }
}
