XPOS EQU #44

IF XPOS!=#44
   xor a
   ret
ELSE
   jp 0
ENDIF