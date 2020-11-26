import socket
import json
dataS=socket.socket()
ip="192.168.1.100"
port=10000
a=0
b=0
dataS.connect((ip,port))
while True:
    try:
        rv=dataS.recv(2400)
        rvstr=rv.decode()
        status=json.loads(rvstr)
        if status["torqsensor"][1][2][2]==999 :
            print("touched!!!!!!!")
        else:
            value=""
            for i in range(3,4):
                value=value+" "+str(status["torqsensor"][1][2][i])
            print(value)
            print(rvstr)
            a=a+1
            print(a)
    except Exception as e:
        print(e)
        b=b+1
        print(b)
        

