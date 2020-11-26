#!/usr/bin/env python
# -*- coding:utf-8 -*-

import socket
import traceback
import time
import json
import csv
import math
import datetime
import jkzuc

def creat_prog(dic):
	with open('second.ngc','w') as f1:
		f1.writelines("#<vel> = "+str(dic['speed'])+"\n#<acc> = "+str(dic['acc'])+"\n#<tol> = 0.5\n#<do> = "+str(dic['do'])+"\n#<begindelay> = "+str(dic['begin_delay'])+"\n#<enddelay> = "+str(dic['end_delay'])+"\n\n")
		f1.writelines("#<pos> = {"+str(dic['points'][0][0])+","+str(dic['points'][0][1])+","+str(dic['points'][0][2]+dic['height'])+","+str(dic['points'][0][3])+","+str(dic['points'][0][4])+","+str(dic['points'][0][5])+"}\n")
		f1.writelines("movl(#<pos>, 0, #<vel>, #<acc>, #<tol>)\n")
		f1.writelines("#<pos> = {"+str(dic['points'][0][0])+","+str(dic['points'][0][1])+","+str(dic['points'][0][2])+","+str(dic['points'][0][3])+","+str(dic['points'][0][4])+","+str(dic['points'][0][5])+"}\n")
		f1.writelines("movl(#<pos>, 0, #<vel>, #<acc>, #<tol>)\n")
		f1.writelines("setdout(0,#<do>,1,1)\n")
		f1.writelines("sleep(#<begindelay>)\n\n")
		for point in dic['points']:
			str0="#<pos> = {"+str(point[0])+","+str(point[1])+","+str(point[2])+","+str(point[3])+","+str(point[4])+","+str(point[5])+"}"
			str1="movl(#<pos>, 0, #<vel>, #<acc>, #<tol>)"
			f1.writelines(str0+"\n")
			f1.writelines(str1+"\n\n")
		f1.writelines("sleep(#<enddelay>)\n")
		f1.writelines("setdout(0,#<do>,0,1)\n")
		f1.writelines("#<pos> = {"+str(dic['points'][-1][0])+","+str(dic['points'][-1][1])+","+str(dic['points'][-1][2]+dic['height'])+","+str(dic['points'][-1][3])+","+str(dic['points'][-1][4])+","+str(dic['points'][-1][5])+"}\n")
		f1.writelines("movl(#<pos>, 0, #<vel>, #<acc>, #<tol>)\n")
		f1.writelines("M2\n")


def creat_first(dic,x,y):
	with open('first.ngc','w') as f1:
		f1.writelines("#<vel> = "+str(dic['speed'])+"\n#<acc> = "+str(dic['acc'])+"\n#<tol> = 0.5\n\n")
		f1.writelines("#<pos> = {"+str(x)+","+str(y)+","+str(dic['points'][0][2])+","+str(dic['points'][0][3])+","+str(dic['points'][0][4])+","+str(dic['points'][0][5])+"}\n")
		f1.writelines("movl(#<pos>, 0, #<vel>, #<acc>, #<tol>)\n")
		f1.writelines("M2\n")
		print "creat_first_succeed"
		print x
		print y


def run_prog(filePath):
	print "run"+filePath
	if __name__ == '__main__':
	    c.teleop_enable(0)
	    c.wait_complete()
	    s.poll()
	    for jnum in range(0, 6):
		if not (s.homed[jnum]):
		    c.home(-1)
		    c.wait_complete()
		    break
	    # open the file
	    c.task_plan_synch()
	    c.wait_complete()
	    c.program_open(filePath)
	    # ensure the mode
	    s.poll()
	    if s.task_mode != jkzuc.MODE_AUTO:
			c.mode(jkzuc.MODE_AUTO)  # task_mode
			c.wait_complete()
			s.poll()
	    c.auto(jkzuc.AUTO_RUN, 0)
	    time.sleep(0.5)
	    s.poll()
            while s.task_mode ==jkzuc.MODE_AUTO:
	    	time.sleep(0.5)
	    	s.poll()
	    print "run complete"




def run_first():
	if __name__ == '__main__':
	    c.teleop_enable(0)
	    c.wait_complete()
	    s.poll()
	    for jnum in range(0, 6):
		if not (s.homed[jnum]):
		    c.home(-1)
		    c.wait_complete()
		    break
	    # open the file
	    c.task_plan_synch()
	    c.wait_complete()
	    filePath = '/home/jakauser/first.ngc'
	    c.program_open(filePath)
	    # ensure the mode
	    s.poll()
	    if s.task_mode != jkzuc.MODE_AUTO:
			c.mode(jkzuc.MODE_AUTO)  # task_mode
			c.wait_complete()
			s.poll()
	    c.auto(jkzuc.AUTO_RUN, 0)



def sendmsg():
	s.poll()
	list0=s.position
	str01="100,"+str(list0[0])+","+str(list0[1])+","+str(list0[2])+","+str(list0[3])+","+str(list0[4])+","+str(list0[5])
	dataSocket.sendall(str01.encode())


def wait_move():
	while True:
		recData=dataSocket.recv(51200)
		Data=recData.decode()
		print Data
		Data = Data.replace('\r', '\\r').replace('\n', '\\n')
		dic = json.loads(Data,strict=False) #得到字典
		if dic['command']=='move':
			print '接收到数据'
		creat_prog(dic)
		time.sleep(0.1)
		filePath = '/home/jakauser/second.ngc'
		print "##########curve"
		run_prog(filePath)
		#time.sleep(10)
		if(dic['complete']==0):
			dataSocket.sendall("NEXT".encode())
		else:
			break


def arraymove(dic):
	x0=dic['points'][0][0]
	y0=dic['points'][0][1]
	x1=dic['points'][1][0]
	y1=dic['points'][1][1]
	x2=dic['points'][2][0]
	y2=dic['points'][2][1]
	num01=dic['num01']
	num12=dic['num12']

	dx0=x1-x0
	dy0=y1-y0

	dx1=x2-x1
	dy1=y2-y1
	for b in range(0,num12):
		for a in range(0,num01):
			if num01!=1:
				if num12==1:
					x=x0+a*dx0/(num01-1)
					y=y0+a*dy0/(num01-1)
				else:
					x=x0+a*dx0/(num01-1)+b*dx1/(num12-1)
					y=y0+a*dy0/(num01-1)+b*dy1/(num12-1)
				print x
				print y
				creat_first(dic,x,y)
				time.sleep(0.1)
				filePath = '/home/jakauser/first.ngc'
				print "############array"
				run_prog(filePath)
				#time.sleep(5)
				sendmsg()
				wait_move()
			else:
				x=x0
				y=y0
				print x
				print y
				creat_first(dic,x,y)
				time.sleep(0.1)
				filePath = '/home/jakauser/first.ngc'
				print "############array"
				run_prog(filePath)
				#time.sleep(5)
				sendmsg()
				wait_move()	

			


c = jkzuc.command()
s = jkzuc.stat()
listenSocket = socket.socket()         # 创建 socket 对象
host = socket.gethostname() # 获取本地主机名
print host
port = 12345      # 设置端口
listenSocket.bind(('192.168.218.132', port))
listenSocket.listen(10)
print 'Waitting for connect...'
dataSocket,addr = listenSocket.accept()     # 建立客户端连接
print '连接成功'
dataSocket.sendall("OK".encode())



while True:
	recData=dataSocket.recv(51200)
	Data=recData.decode()
	print Data
	Data = Data.replace('\r', '\\r').replace('\n', '\\n')
	dic = json.loads(Data,strict=False) #得到字典
	if dic['command']=='connect_test':
		dataSocket.send('yes'.encode())
	elif dic['command']=='array':
		print '接收到数据,array'
		arraymove(dic)
	elif dic['command']=='move':
		print '接收到数据,move'

		creat_prog(dic)
		time.sleep(0.1)
		filePath = '/home/jakauser/second.ngc'
		run_prog(filePath)

dataSocket.close()
listenSocket.close()
print "play end!"
#sock.close()
