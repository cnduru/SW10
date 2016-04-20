#directory "/home/kristian/.opam/4.01.0/lib/extlib";;
#directory "/home/kristian/.opam/4.01.0/lib/camlzip";;
#directory "/home/kristian/.opam/4.01.0/lib/ptrees";;
#directory "/home/kristian/.opam/4.01.0/lib/javalib";;
#directory "/home/kristian/.opam/4.01.0/lib/sawja";;

#directory "/home/kristian/.opam/4.01.0/lib/zip";;


#load "ptrees.cma";;
#load "extLib.cma";;
#load "str.cma";;
#load "unix.cma";;
#load "zip.cma";;
#load "javalib_pack.cmo";;
#load "sawja_pack.cmo";;

open Javalib_pack
open Javalib
open JBasics
open Sawja_pack
open JProgram;;



let (prta,instantiated_classes) =
  JRTA.parse_program 
    "java/sample.jar:rt.jar:jce.jar"
    (JBasics.make_cms (JBasics.make_cn "Sample") JProgram.main_signature);;



(* p_class annots a class, saying if it may be instantiated
   or not. *)
let p_class =
  (fun cn ->
    let ioc = get_node prta cn in
      match ioc with
       | Class _c ->
         if ClassMap.mem (get_name ioc) instantiated_classes then
           match super_class ioc with 
            | Some _S -> [(cn_name (get_name  _S)) ^ " Instantiated"]
            | _ -> ["Instantiated"] else ["Not instantiatied"]
       | _ -> []
  );;

(* p_method annots a method, saying if it is concrete or abstract,
   and if it has been parsed or not (by RTA). *)
let p_method =
  (fun cn ms ->
    let m = get_method (get_node prta cn) ms in
       match m with
        | AbstractMethod _ -> ["Abstract Method"]
        | ConcreteMethod _cm ->
          let cms = make_cms cn ms in
          let parse =
            if ClassMethodMap.mem cms prta.parsed_methods then
              "Parsed" else "Not parsed" in
            ["Concrete Method "; parse]
  )

(* There is no field annotation. *)
let p_field = (fun _ _ -> [])

(* There is no program point annotation. *)
let p_pp = (fun _ _ _ -> [])

(* This is the info type. *)
let simple_info = 
  { JPrintHtml.p_class = p_class;
    JPrintHtml.p_field = p_field;
    JPrintHtml.p_method = p_method;
    JPrintHtml.p_pp = p_pp };;

let output = "./prta"
let () = JPrintHtml.JCodePrinter.print_program ~info:simple_info prta output;;

store_callgraph (get_callgraph prta) "call_graph.txt";;
