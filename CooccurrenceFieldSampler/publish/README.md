# CooccurrenceFieldSampler

2025 by Jan Oliver Rüdiger

To use the CFS tool, follow these steps:

1. Unzip the ZIP file containing the necessary files.
2. For Windows, Linux, and macOS, you will find precompiled binaries that run exclusively on x64 processors.
3. If you are using a different processor type, such as ARM or ARM64, please use the Universal folder.
4. Windows users should run "cfs.exe" directly.
5. For Linux and macOS users, you must mark the cfs file as executable.
6. If using the Universal version, ensure .NET 10.0 is installed for compiling. You can then run the program with "dotnet cfs.dll".
7. To display help information, use the --help parameter.

Help/Parameter:

  --from               (Default: cec / recommended: cec) import file format (valid: cec, bnc, catma, clan, conll, cora, cwd, dewac, dta, folia, fln, korap, leipzig, xces,
                       relannis, salt, json, sketch, speedy, tiger, tlv, treetagger, tsv, txm, weblicht)

  --input              (Default: input/) folder with input-files (mix per file)

  --to                 (Default: cec / recommended: cec) export file format (valid: cec, catma, conll, cwd, csv, dta, folia, i5, korap, xces, plain, salt, json, sketch,
                       speedy, tlv, tsv, treetagger, txm, weblicht)

  --layer              (Default: Wort) use this layer to calculate the co-occurrences

  --output             (Default: output.cec6) output file (every round and logfile)

  --minFrequency       (Default: 1 / recommended: 5) min. absolute frequency

  --minSignificance    (Default: 1.0 / recommended: 1.0) min. significance (poisson distribution)

  --minChangeRate      (Default: 0.1 / recommended: 0.1) min. significance (poisson distribution)

  --maxRounds          (Default: 10 / recommended: 5) min. absolute frequency

  --help               Display this help screen.

  --version            Display version information.
  
Formats:
cec - CorpusExplorer Corpus (v6) - http://corpusexplorer.de
bnc - British National Corpus - http://www.natcorp.ox.ac.uk/
catma - CATMA (Computer assisted text markup and analysis) - https://catma.de/
clan - CLAN/CHILDES - https://talkbank.org/childes/
conll - CoNLL-U https://universaldependencies.org/format.html
cora - CORA XML - https://cora.readthedocs.io/en/latest/coraxml/
cwd - IMS Open Corpus Workbench (CWB) - https://cwb.sourceforge.io/
dewac - https://wacky.sslmit.unibo.it/doku.php?id=corpora
dta - DTA TCF-XML - https://www.deutschestextarchiv.de/download
folia - FoLiA XML - https://proycon.github.io/folia/
fln - Folker/OrthoNormal - https://exmaralda.org/de/folker-de/
korap - KorAP - http://korap.ids-mannheim.de/
leipzig - Wortschatz Leipzig - https://wortschatz.uni-leipzig.de/en/download/
xces - XCes XML - http://www.xces.org/ / https://www.cs.vassar.edu/CES/
relannis - https://corpus-tools.org/annis/
salt - https://corpus-tools.org/archive-2015-2025/salt/
json - https://de.wikipedia.org/wiki/JSON
sketch - SketchEngine VERT - https://www.sketchengine.eu/glossary/vertical-file/
speedy - SPEEDy Annotation Editor - http://kups.ub.uni-koeln.de/id/eprint/55224
tiger - TiGER-XML - https://www.ims.uni-stuttgart.de/documents/ressourcen/werkzeuge/tigersearch/doc/html/TigerXML.html
tlv - TLV-XML 
treetagger - TreeTagger - https://www.cis.uni-muenchen.de/~schmid/tools/TreeTagger/
tsv - Tab-separated values - https://en.wikipedia.org/wiki/Tab-separated_values
txm - TXM - https://txm.gitpages.huma-num.fr/textometrie/?lang=en
weblicht - Weblicht - https://weblicht.sfs.uni-tuebingen.de/weblichtwiki/Main_Page.html
csv - Comma-separated values - https://en.wikipedia.org/wiki/Comma-separated_values
i5 - i5-XML - https://www.ids-mannheim.de/en/digspra/pb-s1/projects/corpus-development/ids-text-model/
plain - Plaintext - https://en.wikipedia.org/wiki/Plain_text