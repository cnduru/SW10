from HTMLParser import HTMLParser
from htmlentitydefs import name2codepoint

class MyHTMLParser(HTMLParser):
    curTag = ""
    
    def handle_starttag(self, tag, attrs):
        if tag == "li":
            #print "Start tag:", tag
            for attr in attrs:
                if attr [1].isdigit():
                    print "     attr:", attr[1]
        if tag == "span":
            for attr in attrs:
                print attr
    def handle_data(self, data):
        if True:
            print "Data     :", data
    def handle_endtag(self, tag):
        if False:
            print "End tag  :", tag

            
html = open('example.html', 'r').read()

parser = MyHTMLParser()
parser.feed(html)
