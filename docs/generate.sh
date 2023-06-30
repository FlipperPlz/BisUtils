#! /bin/sh

rm -rf html/*

doxygen BisUtilsDoxygen.cfg 2> BisUtilsDoxygen.log

# HTML Help Workshop might need replacement according to Linux system equivalent
hhc html/index.hhp
mv html/index.chm DayZ_ProgDoc.chm
# Display the .chm file
xchm BisUtils_Doc.chm
