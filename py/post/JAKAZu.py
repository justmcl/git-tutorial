import json
import string
import random
import uuid


class JAKAZuProg:
    def __init__(self):
        self.next = {"id": ""}
        # self.clickflag={   # as a block instatnce?
        #     "id": 'flag'+uuid.uuid4(),
        #     "opcode": "event_whenflagclicked",
        #     "inputs": { },
        #     "fields": { },
        #     "next": self.next['id'],
        #     "topLevel": True,
        #     "parent": None,
        #     "shadow": False,
        #     "disabled": False,
        #     "x": 148,
        #     "y": -222
        # }
        self.clickflag = BlockHead()
        self.clickflag.opcode = 'event_whenflagclicked'
        self.clickflag.topLevel = True
        self.clickflag.id = self.assign_id()
        self.blocks = {}
        self.instructs = [self.clickflag]
        self.Stage = {
            "id": self.assign_id(),
            "name": "Stage",
            "isStage": True,
            "x": 0,
            "y": 0,
            "size": 100,
            "direction": 90,
            "draggable": False,
            "currentCostume": 0,
            "costumeCount": 0,
            "visible": True,
            "rotationStyle": "all around",
            "blocks": {},
            "variables": {},
            "lists": {},
            "costumes": [],
            "sounds": []
        }
        self.Sprite1 = {
            "id": self.assign_id(),
            "name": "Sprite1",
            "isStage": False,
            "x": 0,
            "y": 0,
            "size": 100,
            "direction": 90,
            "draggable": False,
            "currentCostume": 0,
            "costumeCount": 0,
            "visible": True,
            "rotationStyle": "all around",
            "blocks": self.blocks,
            "variables": {},
            "lists": {},
            "costumes": [],
            "sounds": []
        }
        self.target = [self.Stage, self.Sprite1]
        self.md5 = 'md5'
        self.meta = {"programVer": 4, "semver": "3.0.0", "vm": "0.1.0", "programMD5": self.md5,
                     "agent": "Mozilla/5.0 (Linux; Android 8.1.0; MI PAD 4 Build/OPM1.171019.019; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/62.0.3202.84 Safari/537.36"}
        self.Zu = {"targets": self.target, "meta": self.meta}
        self.last_block = self.clickflag

    def assign_id(self, append=False, block=None):
        u = uuid.uuid4()
        if append:
            self.last_block.next = u
            self.last_bock = block
        return u.hex

    def append_ins(self, instruct):
        '''append new instruction, 
        1  get a id for the new instruction
        2  assign the next id of the last instruct to this id
        3  append to the internal instructs  list'''
        instruct.id = self.assign_id()
        parentId = self.instructs[-1].id
        self.instructs[-1].next = instruct.id
        self.instructs.append(instruct)
        self.instructs[-1].parent = parentId

    def updatemd5(self, newmd5):
        self.md5 = newmd5

    def tojsons(self):
        for instruct in self.instructs:
            # print(instruct.__dict__)
            if isinstance(instruct, ZuBlock):
                self.blocks[instruct.id] = instruct.__dict__
            elif isinstance(instruct, ZuInstruction):
                for block in instruct.block_list:
                    if instruct.block_list.index(block) == 0:
                        block.parent = instruct.parent
                        if self.instructs.index(instruct) != len(self.instructs) - 1:
                            block.next = instruct.next
                    self.blocks[block.id] = block.__dict__
            else:
                raise Exception
        return json.dumps(self.Zu)

    def tojsonf(self, s):
        if isinstance(s, ZuBlock):
            return ZuBlock.__dict__
        else:
            return json.dumps(s)


class ZuBlock:
    def __init__(self, parent_prog=None):
        if parent_prog is None:
            self.id = 0
        # elif isinstance(parent_prog,JAKAZuProg):
        #    self.id=parent_prog.assign_id()
        else:
            raise Exception
        self.opcode = ""
        self.inputs = {}
        self.fields = {}
        self.next = None
        self.topLevel = False
        self.parent = None
        self.shadow = False
        self.disabled = False


class BlockHead(ZuBlock):
    def __init__(self, x=148, y=-222):
        super().__init__()
        self.x = x
        self.y = y


class ZuInstruction():
    def __init__(self, blocknum=1):
        self.block_list = [ZuBlock() for x in range(blocknum)]


class ZuMoveL(ZuInstruction):

    def __init__(self, pos, joints, vel_degs, acc_degss, tol=0, rel_flag=0, endcond=[-1, 0, 0], point_name="新点1"):
        super().__init__(3)
        self._id = 0
        self.mainbody_init()
        self.objtext_init(point_name)
        self.text_init(pos, joints, vel_degs, acc_degss, tol, rel_flag, endcond)

    @property
    def id(self):
        return self._id

    @id.setter
    def id(self, value):
        self._id = value
        self.mainbody.id = self.objtext.parent = self.text.parent = self._id
        self.objtext.id = 'O1' + self._id
        self.text.id = 'O2' + self._id
        self.mainbody.inputs = {'Name': {'name': 'Name',
                                         'block': self.objtext.id,
                                         'shadow': self.objtext.id,
                                         'disabled': 'false'},
                                'MoveInfos': {'name': 'MoveInfos',
                                              'block': self.text.id,
                                              'shadow': self.text.id,
                                              'disabled': 'true'}}

    def mainbody_init(self):
        self.mainbody = self.block_list[0]
        self.mainbody.opcode = 'jaka_move_line'

    def objtext_init(self, point_name):
        self.objtext = self.block_list[1]
        self.objtext.opcode = 'obj_text'
        self.objtext.shadow = True
        self.objtext.fields = {"TEXT": {'name': 'TEXT', 'value': point_name}}

    def text_init(self, pos, joints, vel_degs, acc_degss, tol=0, rel_flag=0, endcond=[-1, 0, 0]):
        self.text = self.block_list[2]
        self.text.opcode = 'text'
        self.text.shadow = True
        self.text.disabled = "true"
        valueStr = '{0},{1},{2},{3},{4},{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12},{13},{14}'.format(
            joints[0], joints[1], joints[2], joints[3], joints[4], joints[5], pos,
            vel_degs, acc_degss, tol, rel_flag, -1, endcond[0], endcond[1], endcond[2])
        # print(valueStr)
        self.text.fields = {"TEXT": {'name': 'TEXT', 'value': valueStr}}


class ZuMoveJ(ZuInstruction):

    def __init__(self, pos, joints, vel_degs, acc_degss, tol=0, rel_flag=0, endcond=[-1, 0, 0], point_name="新点1"):
        super().__init__(3)
        self._id = 0
        self.mainbody_init()
        self.objtext_init(point_name)
        self.text_init(pos, joints, vel_degs, acc_degss, tol, rel_flag, endcond)

    @property
    def id(self):
        return self._id

    @id.setter
    def id(self, value):
        self._id = value
        self.mainbody.id = self.objtext.parent = self.text.parent = self._id
        self.objtext.id = 'O1' + self._id
        self.text.id = 'O2' + self._id
        self.mainbody.inputs = {'Name': {'name': 'Name',
                                         'block': self.objtext.id,
                                         'shadow': self.objtext.id,
                                         'disabled': 'false'},
                                'MoveInfos': {'name': 'MoveInfos',
                                              'block': self.text.id,
                                              'shadow': self.text.id,
                                              'disabled': 'true'}}

    def mainbody_init(self):
        self.mainbody = self.block_list[0]
        self.mainbody.opcode = 'jaka_move_join'

    def objtext_init(self, point_name):
        self.objtext = self.block_list[1]
        self.objtext.opcode = 'obj_text'
        self.objtext.shadow = True
        self.objtext.fields = {"TEXT": {'name': 'TEXT', 'value': point_name}}

    def text_init(self, pos, joints, vel_degs, acc_degss, tol=0, rel_flag=0, endcond=[-1, 0, 0]):
        self.text = self.block_list[2]
        self.text.opcode = 'text'
        self.text.shadow = True
        self.text.disabled = "true"
        valueStr = '{0},{1},{2},{3},{4},{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12},{13},{14}'.format(
            joints[0], joints[1], joints[2], joints[3], joints[4], joints[5], pos,
            vel_degs, acc_degss, tol, rel_flag, -1, endcond[0], endcond[1], endcond[2])
        # print(valueStr)
        self.text.fields = {"TEXT": {'name': 'TEXT', 'value': valueStr}}


class ZuMoveC(ZuInstruction):

    def __init__(self, pos1, joints1, pos2, joints2, vel_degs, acc_degss, tol=0, rel_flag=0, endcond=[-1, 0, 0],
                 point1_name="圆弧点1", point2_name="圆弧点2"):
        super().__init__(4)
        self._id = 0
        self.mainbody_init()
        self.objtext1_init(point1_name)
        self.objtext2_init(point2_name)
        self.text_init(pos1, joints1, pos2, joints2, vel_degs, acc_degss, tol, rel_flag, endcond)

    @property
    def id(self):
        return self._id

    @id.setter
    def id(self, value):
        self._id = value
        self.mainbody.id = self.objtext1.parent = self.objtext2.parent = self.text.parent = self._id
        self.objtext1.id = 'O1' + self._id
        self.objtext2.id = 'O2' + self._id
        self.text.id = 'O3' + self._id
        self.mainbody.inputs = {'Point1': {'name': 'Point1',
                                           'block': self.objtext1.id,
                                           'shadow': self.objtext1.id,
                                           'disabled': 'false'},
                                'Point2': {'name': 'Point2',
                                           'block': self.objtext2.id,
                                           'shadow': self.objtext2.id,
                                           'disabled': 'false'},
                                'MoveInfos': {'name': 'MoveInfos',
                                              'block': self.text.id,
                                              'shadow': self.text.id,
                                              'disabled': 'true'}}

    def mainbody_init(self):
        self.mainbody = self.block_list[0]
        self.mainbody.opcode = 'jaka_move_circle'

    def objtext1_init(self, point_name):
        self.objtext1 = self.block_list[1]
        self.objtext1.opcode = 'obj_text'
        self.objtext1.shadow = True
        self.objtext1.fields = {"TEXT": {'name': 'TEXT', 'value': point_name}}

    def objtext2_init(self, point_name):
        self.objtext2 = self.block_list[2]
        self.objtext2.opcode = 'obj_text'
        self.objtext2.shadow = True
        self.objtext2.fields = {"TEXT": {'name': 'TEXT', 'value': point_name}}

    def text_init(self, pos1, joints1, pos2, joints2, vel_degs, acc_degss, tol=0, rel_flag=0, endcond=[-1, 0, 0]):
        self.text = self.block_list[3]
        self.text.opcode = 'text'
        self.text.shadow = True
        self.text.disabled = "true"
        valueStr = '{0},{1},{2},{3},{4},{5}|{6}|{7},{8},{9},{10},{11},{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19},{20},{21}'.format(
            joints1[0], joints1[1], joints1[2], joints1[3], joints1[4], joints1[5], pos1,
            joints2[0], joints2[1], joints2[2], joints2[3], joints2[4], joints2[5], pos2,
            vel_degs, acc_degss, tol, rel_flag, -1, endcond[0], endcond[1], endcond[2])
        # print(valueStr)
        self.text.fields = {"TEXT": {'name': 'TEXT', 'value': valueStr}}


class ZuUcs(ZuInstruction):

    def __init__(self, pos, ucs_name="新用户坐标系"):
        super().__init__(3)
        self._id = 0
        self.mainbody_init()
        self.objtext_init(ucs_name)
        self.text_init(pos)

    @property
    def id(self):
        return self._id

    @id.setter
    def id(self, value):
        self._id = value
        self.mainbody.id = self.objtext.parent = self.text.parent = self._id
        self.objtext.id = 'O1' + self._id
        self.text.id = 'O2' + self._id
        self.mainbody.inputs = {'UcsName': {'name': 'UcsName',
                                         'block': self.objtext.id,
                                         'shadow': self.objtext.id,
                                         'disabled': 'false'},
                                'UcsInfos': {'name': 'UcsInfos',
                                              'block': self.text.id,
                                              'shadow': self.text.id,
                                              'disabled': 'true'}}

    def mainbody_init(self):
        self.mainbody = self.block_list[0]
        self.mainbody.opcode = 'jaka_set_ucs'

    def objtext_init(self, ucs_name):
        self.objtext = self.block_list[1]
        self.objtext.opcode = 'obj_text'
        self.objtext.shadow = True
        self.objtext.fields = {"TEXT": {'name': 'TEXT', 'value': ucs_name}}

    def text_init(self, pos):
        self.text = self.block_list[2]
        self.text.opcode = 'text'
        self.text.shadow = True
        self.text.disabled = "true"
        self.text.fields = {"TEXT": {'name': 'TEXT', 'value': pos}}


class ZuTcp(ZuInstruction):

    def __init__(self, pos, tcp_name="新TCP"):
        super().__init__(3)
        self._id = 0
        self.mainbody_init()
        self.objtext_init(tcp_name)
        self.text_init(pos)

    @property
    def id(self):
        return self._id

    @id.setter
    def id(self, value):
        self._id = value
        self.mainbody.id = self.objtext.parent = self.text.parent = self._id
        self.objtext.id = 'O1' + self._id
        self.text.id = 'O2' + self._id
        self.mainbody.inputs = {'TcpName': {'name': 'TcpName',
                                         'block': self.objtext.id,
                                         'shadow': self.objtext.id,
                                         'disabled': 'false'},
                                'TcpInfos': {'name': 'TcpInfos',
                                              'block': self.text.id,
                                              'shadow': self.text.id,
                                              'disabled': 'true'}}

    def mainbody_init(self):
        self.mainbody = self.block_list[0]
        self.mainbody.opcode = 'jaka_set_tcp'

    def objtext_init(self, tcp_name):
        self.objtext = self.block_list[1]
        self.objtext.opcode = 'obj_text'
        self.objtext.shadow = True
        self.objtext.fields = {"TEXT": {'name': 'TEXT', 'value': tcp_name}}

    def text_init(self, pos):
        self.text = self.block_list[2]
        self.text.opcode = 'text'
        self.text.shadow = True
        self.text.disabled = "true"
        self.text.fields = {"TEXT": {'name': 'TEXT', 'value': pos}}
