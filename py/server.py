#!/usr/bin/python
# -*- coding: UTF-8 -*-
# 文件名：server.py
 
import socket               # 导入 socket 模块
import json
 
listenSocket = socket.socket()         # 创建 socket 对象
host = socket.gethostname() # 获取本地主机名
port = 12345                # 设置端口
listenSocket.bind((host, port))        # 绑定端口
 
listenSocket.listen(10)
dataSocket,addr = listenSocket.accept()     # 建立客户端连接
print ('连接地址：', addr)
##msg = '欢迎！'  #strip默认取出字符串的头尾空格
##dataSocket.send(msg.encode())

while True:
    recData=dataSocket.recv(51200)
    Data=recData.decode()
    dic = json.loads(Data) #得到字典
    for point in dic['points']:
        print(point)#打印收到的400组坐标
    print(f'收到信息{Data}')
    #dataSocket.send(f'服务端接收到了信息: {Data}'.encode())
dataSocket.close()
lisenSocket.close()
