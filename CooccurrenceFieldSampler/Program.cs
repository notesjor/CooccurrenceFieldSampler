using System;
using System.Collections.Generic;
using CommandLine;

namespace CooccurrenceFieldSampler
{
  internal class Program
  {
    public class Options
    {
      [Option("from", Default = "Cec6", Required = false, HelpText = "import file format (valid: )")]
      public string FromFormat { get; set; }

      [Option("input", Default = "input/", Required = false, HelpText = "folder with input-files (mix per file)")]
      public string FromPath { get; set; }

      [Option("to", Default = "Cec6", Required = false, HelpText = "export file format (valid: )")]
      public string ToFormat { get; set; }

      [Option("layer", Default = "Wort", Required = false, HelpText = "use this layer to calculate the co-occurrences")]
      public string LayerName { get; set; }

      [Option("output", Default = "output/", Required = false, HelpText = "output path (every round and logfile)")]
      public string ToPath { get; set; }

      [Option("minFrequency", Default = 1, Required = false, HelpText = "min. absolute frequency")]
      public int MinFrequency { get; set; }

      [Option("minSignificance", Default = 1.0, Required = false, HelpText = "min. significance (poisson distribution)")]
      public double MinSignificance { get; set; }

      [Option("minChangeRate", Default = 0.0, Required = false, HelpText = "min. significance (poisson distribution)")]
      public double MinChangeRate { get; set; }

      [Option("maxRounds", Default = 10, Required = false, HelpText = "min. absolute frequency")]
      public int MaxRounds { get; set; }
    }

    static void Main(string[] args)
    {
      var parserResult = Parser.Default.ParseArguments<Options>(args);

      parserResult
        .WithParsed(RunOptions)
        .WithNotParsed(errs => HandleParseError(parserResult, errs));
    }

    static void RunOptions(Options opts)
    {
      //handle options
    }

    static void HandleParseError(ParserResult<Options> result, IEnumerable<Error> errs)
    {
      Console.WriteLine("Error parsing command line arguments.");
      Console.WriteLine("please use: cfs --help to get more information.");
    }
  }
}
