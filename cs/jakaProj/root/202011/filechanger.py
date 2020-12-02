with open("log0.cs") as target:
    lines=target.readlines()
with open("log00.cs","w") as cs:
    for line in lines:
        if line!="\n":
            cs.writelines(line)