using CommandLine;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Ecosystem;
using CorpusExplorer.Sdk.Extern.Json.SimpleStandoff;
using CorpusExplorer.Sdk.Extern.Json.Speedy;
using CorpusExplorer.Sdk.Extern.Plaintext.ClanChildes;
using CorpusExplorer.Sdk.Extern.Plaintext.LeipzigerWortschatz;
using CorpusExplorer.Sdk.Extern.Plaintext.RelAnnis;
using CorpusExplorer.Sdk.Extern.Plaintext.Tsv;
using CorpusExplorer.Sdk.Extern.Xml.Bnc;
using CorpusExplorer.Sdk.Extern.Xml.Catma._5._0;
using CorpusExplorer.Sdk.Extern.Xml.Catma._6._0;
using CorpusExplorer.Sdk.Extern.Xml.CoraXml._1._0;
using CorpusExplorer.Sdk.Extern.Xml.Dewac;
using CorpusExplorer.Sdk.Extern.Xml.Dta.Tcf2017;
using CorpusExplorer.Sdk.Extern.Xml.FoLiA;
using CorpusExplorer.Sdk.Extern.Xml.Folker.Fln;
using CorpusExplorer.Sdk.Extern.Xml.Ids.I5Xml;
using CorpusExplorer.Sdk.Extern.Xml.Ids.KorAP;
using CorpusExplorer.Sdk.Extern.Xml.Opus;
using CorpusExplorer.Sdk.Extern.Xml.SaltXml;
using CorpusExplorer.Sdk.Extern.Xml.Tiger.Importer;
using CorpusExplorer.Sdk.Extern.Xml.Txm;
using CorpusExplorer.Sdk.Extern.Xml.Weblicht;
using CorpusExplorer.Sdk.Model.Adapter.Corpus;
using CorpusExplorer.Sdk.Model.Adapter.Layer;
using CorpusExplorer.Sdk.Model.Cache;
using CorpusExplorer.Sdk.Utils.CorpusManipulation;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Exporter;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Exporter.Abstract;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Importer;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Importer.Abstract;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Importer.CorpusExplorerV6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Blocks.Cooccurrence;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Model.Extension;

namespace CooccurrenceFieldSampler
{
  internal class Program
  {
    private static Selection _all;
    private static CorpusAdapterWriteDirect? _corpus;
    private static Dictionary<string, AbstractImporter> _importer = new Dictionary<string, AbstractImporter>
    {
      { "cec", new ImporterCec6() },
      { "bnc", new ImporterBnc() },
      { "catma", new ImporterCatma() },
      { "clan", new ImporterClanChildes() },
      { "conll", new ImporterConll() },
      { "cora", new ImporterCoraXml10() },
      { "cwb", new ImporterCorpusWorkBench() },
      { "dewac", new ImporterDewac() },
      { "dta", new ImporterDta2017() },
      { "folia", new ImporterFolia() },
      { "fln", new ImporterFolkerFln() },
      { "korap", new ImporterKorap2021() },
      { "leipzig", new ImporterLeipzigerWortschatz() },
      { "xces", new ImporterOpusXces() },
      { "relannis", new ImporterRelAnnis() },
      { "salt", new ImporterSaltXml() },
      { "json", new ImporterSimpleJsonStandoff() },
      { "sketch", new ImporterSketchEngine() },
      { "speedy", new ImporterSpeedy() },
      { "tiger", new ImporterTiger() },
      { "tlv", new ImporterTlv() },
      { "treetagger", new ImporterTreeTagger() },
      { "tsv", new ImporterTsv() },
      { "txm", new ImporterTxm() },
      { "weblicht", new ImporterWeblicht() },
    };
    private static Dictionary<string, AbstractExporter> _exporter = new Dictionary<string, AbstractExporter>
    {
      { "cec", new ExporterCec6() },
      { "catma", new ExporterCatma() },
      { "conll", new ExporterConll() },
      { "cwb", new ExporterCorpusWorkBench2022() },
      { "csv", new ExporterCsv { ValueSeparator=";" } },
      { "dta", new ExporterDta2017() },
      { "folia", new ExporterFolia() },
      { "i5", new ExporterI5() },
      { "korap", new ExporterKorap() },
      { "xces", new ExporterOpusXces() },
      { "plain", new ExporterPlaintext() },
      { "salt", new ExporterSaltXml() },
      { "json", new ExporterJson() },
      { "sketch", new ExporterSketchEngine() },
      { "speedy", new ExporterSpeedy() },
      { "tlv", new ExporterTlv() },
      { "tsv", new ExporterCsv { ValueSeparator="\t" } },
      { "treetagger", new ExporterTreeTagger() },
      { "txm", new ExporterTxm() },
      { "weblicht", new ExporterWeblicht() },
    };
    private static Random _rnd = new Random();

    public class Options
    {
      [Option("from", Default = "cec", Required = false, HelpText = "import file format (valid: cec, bnc, catma, clan, conll, cora, cwd, dewac, dta, folia, fln, korap, leipzig, xces, relannis, salt, json, sketch, speedy, tiger, tlv, treetagger, tsv, txm, weblicht)")]
      public string FromFormat { get; set; }

      [Option("input", Default = "input/", Required = false, HelpText = "folder with input-files (mix per file)")]
      public string FromPath { get; set; }

      [Option("to", Default = "cec", Required = false, HelpText = "export file format (valid: cec, catma, conll, cwd, csv, dta, folia, i5, korap, xces, plain, salt, json, sketch, speedy, tlv, tsv, treetagger, txm, weblicht)")]
      public string ToFormat { get; set; }

      [Option("layer", Default = "Wort", Required = false, HelpText = "use this layer to calculate the co-occurrences")]
      public string LayerName { get; set; }

      [Option("output", Default = "output.cec6", Required = false, HelpText = "output file (every round and logfile)")]
      public string ToPath { get; set; }

      [Option("minFrequency", Default = 1, Required = false, HelpText = "min. absolute frequency")]
      public int MinFrequency { get; set; }

      [Option("minSignificance", Default = 1.0, Required = false, HelpText = "min. significance (poisson distribution)")]
      public double MinSignificance { get; set; }

      [Option("minChangeRate", Default = 0.1, Required = false, HelpText = "min. significance (poisson distribution)")]
      public double MinChangeRate { get; set; }

      [Option("maxRounds", Default = 10, Required = false, HelpText = "min. absolute frequency")]
      public int MaxRounds { get; set; }
    }

    static void Main(string[] args)
    {
      var parserResult = Parser.Default.ParseArguments<Program.Options>(args);

      parserResult
        .WithParsed(RunOptions)
        .WithNotParsed(errs => HandleParseError(parserResult, errs));
    }

    static void RunOptions(Program.Options opts)
    {
      if (!_importer.ContainsKey(opts.FromFormat.ToLower()))
      {
        Console.WriteLine($"you requested {opts.FromFormat.ToLower()} as 'from' (format) but this import-format is not supported.");
        Console.WriteLine($"use: {string.Join(", ", _importer.Keys)}");
        return;
      }
      var importer = _importer[opts.FromFormat.ToLower()];
      if (!Directory.Exists(opts.FromPath))
      {
        Console.WriteLine($"you set {opts.FromPath} as input directory, but this directory don't exists.");
        return;
      }
      var input = opts.FromPath;
      var files = Directory.GetFiles(input, "*.*", SearchOption.TopDirectoryOnly)?.ToList();
      if (files.Count == 0)
      {
        Console.WriteLine($"the input directory {opts.FromPath} don't contains any files.");
        return;
      }

      if (!_exporter.ContainsKey(opts.ToFormat.ToLower()))
      {
        Console.WriteLine($"you requested {opts.ToFormat.ToLower()} as 'to' (format) but this export-format is not supported.");
        Console.WriteLine($"use: {string.Join(", ", _exporter.Keys)}");
        return;
      }
      var exporter = _exporter[opts.ToFormat.ToLower()];
      var output = opts.ToPath;

      var log = $"{output}.log";
      if (File.Exists(log))
        File.Delete(log);

      CorpusExplorerEcosystem.Initialize(cacheStrategy: new CacheStrategyDisableCaching());
      Configuration.MinimumFrequency = opts.MinFrequency;
      Configuration.SetSignificance(new PoissonSignificance());
      Configuration.MinimumSignificance = opts.MinSignificance;

      PrintInfo(files, opts.LayerName, opts.MinChangeRate, opts.MaxRounds, log);

      // Preparation
      var selections = PrepareCorpusIndex(files, importer, opts.LayerName);

      var corporaCount = selections.Count();
      var sizes = selections.Select(x => x.Sum(y => (long)y.Value)).ToArray();
      // Berechne das Kookkurrenzfeld für die Basis-Korpora
      var baseField = selections.Select(x => GetBaseCooccurrences(opts.LayerName, x.Keys)).ToArray();
      PrintBaseField(baseField);
      var targets = baseField.Select(x => x.Sum(x => x.Value.Length)).ToArray();

      // Ratios - in der ersten Runde immer 1.0
      // Danach werden die Ratios angepasst.
      var ratios = new double[corporaCount];
      for (var i = 0; i < ratios.Length; i++)
        ratios[i] = 1.0;

      List<Guid> guids = null;
      for (var i = 1; i <= opts.MaxRounds; i++)
      {
        PrintRatios(ratios);

        guids = SampleCorpus(ratios, ref sizes, ref selections);
        // Berechne, die Kookkurrenzen für das Sample
        var cooccurrences = GetCooccurrences(opts.LayerName, guids);
        // Zähle, pro Korpus wieviele Kookkurrenzen mit den Gesamt-Kookkurrenzen übereinstimmen
        var matches = GetMatches(cooccurrences, baseField);
        PrintMatches(matches);

        // Berechne eine neue Ratio
        var ratiosNew = Resolve(matches, targets, opts.MinChangeRate, ratios);

        LogResults(log, i, opts.MaxRounds, files, ratios, matches, targets, ratiosNew);

        // Abbruch, wenn Schwelle unterschritten
        // Abbruch, wenn ein Korpus überhaupt nicht mehr gebraucht wird.
        var change = CalcChangeDiff(ratios, ratiosNew);
        Console.WriteLine($"CR: {change}");
        if (change < opts.MinChangeRate || ratiosNew.Any(x => x == 0) || ratiosNew.All(x => (int)x == 1))
        {
          Save(exporter, guids, opts.ToPath);
          return; // Beende Programm
        }

        ratios = ratiosNew;
      }

      Save(exporter, guids, opts.ToPath);
    }

    private static void PrintBaseField(Dictionary<int, int[]>[] baseField)
    {
      for (int i = 0; i < baseField.Length; i++)
        Console.WriteLine($"BF ({i + 1:D2}): {baseField[i].Keys.Count()} >> {baseField[i].Values.Sum(x => x.Length)}");
    }

    private static void PrintRatios(double[] ratios)
    {
      Console.WriteLine($"RATIO: {string.Join("\t", ratios.Select(x => Math.Round(x, 5)))}");
    }

    private static void PrintMatches(int[] matches)
    {
      Console.WriteLine($"MATCH: {string.Join("\t", matches)}");
    }

    private static List<Guid> SampleCorpus(double[] ratios, ref long[] sizes, ref List<Dictionary<Guid, int>> corpora)
    {
      var res = new List<Guid>();

      for (int i = 0; i < corpora.Count; i++)
      {
        var ratio = ratios[i];

        if (ratio >= 1.0)
        {
          res.AddRange(corpora[i].Keys);
          continue;
        }

        var c = corpora[i];
        long size = (long)(sizes[i] * ratio);
        if (size < 1000)
          size = 1000;
        long cnt = 0;

        var input = c.Keys.ToList();
        var output = new List<Guid>();

        while (cnt < size && input.Count > 0)
        {
          var rnds = GetRandomIndices(1000, input.Count);
          var breaks = false;

          foreach (var id in rnds)
          {
            var guid = input[id];
            output.Add(guid);

            cnt += c[guid];
            if (cnt > size)
            {
              breaks = true;
              break;
            }
          }

          if (breaks)
            break;

          input = input.Where((x, j) => !rnds.Contains(j)).ToList();
        }

        res.AddRange(output);
      }

      return res;
    }

    private static List<Dictionary<Guid, int>> PrepareCorpusIndex(List<string> files, AbstractImporter importer, string layer)
    {
      var res = new List<Dictionary<Guid, int>>();

      var merger = new CorpusMerger();
      foreach (var file in files)
      {
        try
        {
          var corpora = importer.Execute(new[] { file });
          foreach (var corpus in corpora)
          {
            foreach (var l in corpus.LayerUniqueDisplaynames.Where(x => x != layer))
              corpus.LayerDelete(l);
            var tmp = ConvertSentenceToDocuments((CorpusAdapterWriteDirect)corpus, layer, out var sizeInfo);
            res.Add(sizeInfo);
            merger.Input(tmp);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"error importing file {file}: {ex.Message}");
        }
      }

      merger.Execute();

      merger.Output.TryDequeue(out var c);
      _corpus = (CorpusAdapterWriteDirect)c;
      _all = _corpus.ToSelection();

      return res;
    }

    private static CorpusAdapterWriteDirect ConvertSentenceToDocuments(CorpusAdapterWriteDirect corpus, string layer, out Dictionary<Guid, int> sizeInfo)
    {
      var l = corpus.GetLayers(layer).First();
      var state = l.ToLayerState();

      var dictGuids = new Dictionary<Guid, Guid>();
      var newDocs = new Dictionary<Guid, int[][]>();
      sizeInfo = new Dictionary<Guid, int>();

      foreach (var dsel in state._documents)
      {
        var oguid = dsel.Key;
        foreach (var s in dsel.Value)
        {
          var size = s.Length;
          if (size < 4)
            continue;
          if (size > 100)
            continue;

          var nguid = Guid.NewGuid();
          sizeInfo.Add(nguid, size);
          dictGuids.Add(nguid, oguid);
          newDocs.Add(nguid, new int[][] { s });
        }
      }

      state._documents = newDocs;
      corpus.LayerDelete(layer);
      corpus.AddLayer(LayerAdapterWriteDirect.Create(state));

      var ometa = corpus.DocumentMetadata.ToDictionary(x => x.Key, x => x.Value);
      var nmeta = new Dictionary<Guid, Dictionary<string, object>>();
      foreach (var g in dictGuids)
      {
        var meta = ometa[g.Value].ToDictionary(x => x.Key, x => x.Value);
        if (meta.ContainsKey("GUID"))
          meta.Remove("GUID");
        if (meta.ContainsKey("RGUID"))
          meta.Remove("RGUID");
        meta.Add("RGUID", g.Value);
        nmeta.Add(g.Key, meta);
      }
      corpus.ResetAllDocumentMetadata(nmeta);
      return corpus;
    }

    private static void Save(AbstractExporter exporter, List<Guid> selection, string outputPath)
    {
      if (selection == null)
        return;

      exporter.Export(_all.CreateTemporary(selection).ToCorpus(), outputPath);
    }

    private static void PrintInfo(List<string> files, string layer, double min, int max, string log)
    {
      Console.WriteLine("ContextEquivalenceSampler - (c) 2025 by Jan Oliver Rüdiger");
      Console.WriteLine($"{files.Count} files - Layer: {layer}");
      Console.WriteLine($"min. change: {min} - max. rounds: {max}");
      Console.WriteLine($"output: {log}");
    }

    private static double CalcChangeDiff(double[] r, double[] n) => r.Select((t, i) => Math.Abs(t - n[i])).Max();

    private static void LogResults(string log, int i, int max, List<string> files, double[] ratios, int[] matches,
                                   int[] targets, double[] ratiosNew)
    {
      if (!File.Exists(log))
        LogHeader(log, files);

      var stb = new List<string>();
      stb.Add(i.ToString());
      stb.Add(max.ToString());
      for (var j = 0; j < files.Count; j++)
      {
        stb.Add(Path.GetFileNameWithoutExtension(files[j]));
        stb.Add(ratios[j].ToString());
        stb.Add(matches[j].ToString());
        stb.Add(targets[j].ToString());
        stb.Add((matches[j] / (double)targets[j] * 100.0).ToString());
        stb.Add(ratiosNew[j].ToString());
      }

      File.AppendAllText(log, string.Join("\t", stb) + "\r\n", Encoding.UTF8);
    }

    private static void LogHeader(string log, List<string> files)
    {
      var stb = new List<string>();
      stb.Add("ROUND");
      stb.Add("ROUND (MAX)");
      for (var j = 0; j < files.Count; j++)
      {
        stb.Add($"FILE ({j})");
        stb.Add($"RATIO - PRE ({j})");
        stb.Add($"MATCHES ({j})");
        stb.Add($"TARGET ({j})");
        stb.Add($"MATCHES% ({j})");
        stb.Add($"RATIO - POST ({j})");
      }

      File.AppendAllText(log, string.Join("\t", stb) + "\r\n", Encoding.UTF8);
    }

    private static double[] Resolve(int[] matches, int[] targets, double min, double[] ratios)
    {
      var factor = new double[matches.Length];
      for (var i = 0; i < factor.Length; i++)
      {
        factor[i] = matches[i] <= 0 ? 0.001 : matches[i] / (double)targets[i];

        if (1.0 - factor[i] < min) // Wenn Abstand kleiner min, dann übernehme alten Wert
          factor[i] = ratios[i];
        else
          factor[i] = ratios[i] + (1.0 - factor[i]);
      }

      // ReSharper disable once InvertIf
      if (factor.Any(x => x > 1.0))
      {
        var max = factor.Max() + min;
        for (var i = 0; i < factor.Length; i++)
          factor[i] /= max;
      }

      return factor;
    }

    private static int[] GetMatches(Dictionary<int, HashSet<int>> cooccurrences, Dictionary<int, int[]>[] baseField)
    {
      var res = new int[baseField.Length];
      for (var i = 0; i < baseField.Length; i++)
      {
        var count = 0;
        var check = baseField[i];

        foreach (var x in check)
          if (cooccurrences.ContainsKey(x.Key))
            count += x.Value.Count(y => cooccurrences[x.Key].Contains(y));
        res[i] = count;
      }
      return res;
    }

    private static Dictionary<int, int[]> GetBaseCooccurrences(string layer, IEnumerable<Guid> selections)
    {
      var select = _all.CreateTemporary(selections);

      var block = select.CreateBlock<CooccurrenceIndexOnlyBlock>();
      block.LayerDisplayname = layer;
      block.Calculate();

      return block.CooccurrenceSignificance.ToDictionary(x => x.Key, x => x.Value.Keys.ToArray());
    }

    private static Dictionary<int, HashSet<int>> GetCooccurrences(string layer, IEnumerable<Guid> selections)
    {
      var select = _all.CreateTemporary(selections);
      var block = select.CreateBlock<CooccurrenceIndexOnlyBlock>();
      block.LayerDisplayname = layer;
      block.Calculate();
      return block.CooccurrenceSignificance.ToDictionary(x => x.Key, x => new HashSet<int>(x.Value.Keys));
    }


    private static int[] GetRandomIndices(int take, int max)
    {
      if (take >= max)
      {
        var tmpD = new int[max];
        for (var i = 0; i < max; i++)
          tmpD[i] = i;
        return tmpD;
      }

      var tmp = new List<int>();
      for (var i = 0; i < max; i++)
        tmp.Add(i);

      var res = new int[max];
      for (var i = 0; i < res.Length; i++)
      {
        var idx = _rnd.Next(0, tmp.Count);
        res[i] = tmp[idx];
        tmp.RemoveAt(idx);
      }

      return res.OrderByDescending(x => x).ToArray();
    }

    static void HandleParseError(ParserResult<Program.Options> result, IEnumerable<Error> errs)
    {
      Console.WriteLine("Error parsing command line arguments.");
      Console.WriteLine("please use: cfs --help to get more information.");
    }
  }
}
