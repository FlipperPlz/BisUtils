deltree /y gen\*.*
"C:\Program Files\doxygen\bin\doxygen" BisUtilsDoxygen.cfg 2> BisUtilsDoxygen.log
"C:\Program Files (x86)\HTML Help Workshop\hhc" gen\html\index.hhp

move /Y gen\html\index.chm BisUtils_ProgDoc.chm
start BisUtils_ProgDoc.chm