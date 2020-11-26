import socket               # 导入 socket 模块
import json
 
dataSocket = socket.socket()         # 创建 socket 对象
host = socket.gethostname() # 获取本地主机名
port = 10000                # 设置端口号
 
dataSocket.connect(("192.168.1.100", port))
while True:
    #toSend=input('??')
    #dataSocket.send(toSend.encode())
    received = dataSocket.recv(512000)
    revstr = received.decode()
    try:
        status=json.loads(revstr)
        for i in range(0,6):
            print (status["torqsensor"][1][2][i])
    except:
        print("bad")
        print(revstr)
s.close()
