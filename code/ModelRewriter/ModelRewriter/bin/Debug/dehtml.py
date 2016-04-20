#Code by xperroni http://stackoverflow.com/users/476920/xperroni
from html.parser import HTMLParser
from re import sub
from sys import stderr
from traceback import print_exc


class _DeHTMLParser(HTMLParser):
    def __init__(self):
        HTMLParser.__init__(self)
        self.__text = []

    def handle_data(self, data):
        text = data.strip()
        if len(text) > 0:
            text = sub('[ \t\r\n]+', '', text)
            self.__text.append(text)

    def handle_starttag(self, tag, attrs):
        if tag == 'li':
            for attr in attrs:
                if attr[1].isdigit():
                    self.__text.append(attr[1] + ": ")

    def handle_startendtag(self, tag, attrs):
        if tag == 'br':
            pass

    def text(self):
        return ''.join(self.__text).strip()


def dehtml(text):
    try:
        parser = _DeHTMLParser()
        parser.feed(text)
        parser.close()
        return parser.text()
    except:
        print_exc(file=stderr)
        return text


def main():
    f = open("Example.html")

    for line in f:
        print(dehtml(line).strip('\n'))
    
    #print(dehtml(text))


if __name__ == '__main__':
    main()
