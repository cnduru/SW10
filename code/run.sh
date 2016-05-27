#!/bin/bash
tag="virtual"
codePath="ocaml/output/"$tag"/"
outPath="ModelRewriter/ModelRewriter/bin/Debug/"$tag"/"
#cls=("ExampleCFI")
cls=("AclassCGI" "BclassCGI" "VirtualCGI")

for c in ${cls[@]}; do         
  python3 ocaml/parser.py $codePath${c}.html $outPath${c}.txt
done
