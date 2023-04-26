# SaltTextTool
# Usage:
> salt_strings.exe -mode <mode> <mode (extract|import)> <json/txt> <input game file> [input txt/json] [output file]
  
Modes:
```
skilltree     Work with skilltree.zsx file
dialog        Work with dialog.zdx file
strings       Work with strings.ztx file
missions      Work with missions.zms file
loot          Work with loot.zls file
monsters      Work with monsters.zms file (dump to JSON only)
```
  
Extraction example:
> salt_strings.exe -mode loot -extract txt loot.zls
  
> salt_strings.exe -mode loot -extract json loot.zls

Importing example:
> salt_strings.exe -mode loot -import txt strings.ztx strings.ztx.txt new\strings.ztx

> salt_strings.exe -mode loot -import txt loot.zls loot.zls.txt new\loot.zls
