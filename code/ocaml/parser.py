from lxml import etree
from sys import argv 

with open(argv[1], 'r') as f:
    doc = f.read()

tree = etree.HTML(doc)
r = tree.xpath("//div[@class='classname']")[0]

#Class Name
output = [r.text.replace("\n","")]

fields = "".join(tree.xpath("//span[@class='field_signature']//text()")).replace("\n","").split(';')
output.extend(fields)

for method in tree.xpath(".//div[@class='method']"):
    r = ""
    for para in method.xpath("./div[@class='method_signature']//text()"):
        r += " " + para
    #Method sig        
    output.append(r.replace("\n", ""))
    for inst in method.xpath(".//ol[@class='code']/li"):
        r = inst.attrib['value'] + ". "
        for para in inst.xpath(".//span"):
            r += " " + para.text
            for a in para.xpath("./a"):
                r += a.text
        #code
        output.append(r.replace("\n", ""))
        
    for excep in method.xpath("./span[@title='Exceptions']//li/span"):
        output.append(excep.text.replace("\n", ""))

with open(argv[2], "w") as f:
    for e in output:
        f.write(e + "\n")
