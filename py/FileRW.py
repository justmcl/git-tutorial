# with open('cv1.csv') as f_o:
#     for lines in f_o:
#         print((" "+lines).rstrip().replace(","," "))

# with open('cv1.csv') as f_o:
#     lines=f_o.readlines()
# with open('ocv1.csv','w') as f1:
#     for line in lines:
#         str=(" "+line).replace(","," ").rstrip()
#         f1.write(str)
#         print((" "+line).replace(","," ").rstrip())

with open('qwe.csv') as f_o:
    lines=f_o.readlines()
with open('sc0.ngc','w') as f1:
    f1.writelines("#<vel> = 100\n#<acc> = 500\n#<tol> = 1\n\n")
    for line in lines:
        str0=("#<pos> = {"+line).rstrip()+",180,0,0}"
        str1="movl(#<pos>, 0, #<vel>, #<acc>, #<tol>)"
        f1.writelines(str0+"\n")
        f1.writelines(str1+"\n\n")
    f1.writelines("M2\n")

