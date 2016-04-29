from lxml import etree
from sys import argv 

with open(argv[1], 'r') as f:
    doc = f.read().replace('<span class="methodname clickable" onclick="showInfoList(this)">', "").replace('</span>', "")

tree = etree.HTML(doc)
r = tree.xpath("//div[@class='classname']")[0]

#Class Name
output = [r.text.replace("\n","")]

for method in tree.xpath(".//div[@class='method']"):
    r = method.xpath(".//div[@class='method_signature']")[0]
    #Method sig        
    output.append(r.text.replace("\n", ""))
    for inst in method.xpath(".//ol/li"):
        r = inst.attrib['value'] + ". "
        r += inst.xpath(".//span")[0].text
        #code
        output.append(r.replace("\n", ""))

with open(argv[2], "w") as f:
    for e in output:
        f.write(e + "\n")






