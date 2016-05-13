#!/bin/bash
tag="simple_purse_cgi"
codePath="ocaml/output/"$tag"/"
outPath="ModelRewriter/ModelRewriter/bin/Debug/"$tag"/"
cls=("ExampleCGI")
#cls=("Aclass" "Bclass" "Virtual")

for c in ${cls[@]}; do         
  python3 ocaml/parser.py $codePath${c}.html $outPath${c}.txt
done
