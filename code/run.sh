#!/bin/bash
tag="simple_purse"
codePath="ocaml/output/"$tag"/"
outPath="ModelRewriter/ModelRewriter/bin/Debug/"$tag"/"
cls=("Example")
#cls=("Aclass" "Bclass" "Virtual")

for c in ${cls[@]}; do         
  python3 ocaml/parser.py $codePath${c}.html $outPath${c}.txt
done
