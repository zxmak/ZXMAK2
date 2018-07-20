1.) Download and install HTML Help workshop(http://download.microsoft.com/download/0/A/9/0A939EF6-E31C-430F-A3DF-DFAE7960D564/htmlhelp.exe)

2.) Open ZXMak2.hhp file(from ZXMak2 archive) in HTML Help workshop and modify the documentation(htm files can be modified by any application or manually)

3.) Compile !

4.) Check ZXMak2.chm and all modified HTML Help workshop files(!!!) into the archive

BUG(Windoze, C#)!!!:
There is a known bug in reading .chm files. In case the path to .chm(here ZXMak2.chm) contains 
one of the following characters: '#', '?', '&', or '+'; then the file will not be correctly read(there will be no right 
pane text visible. Only text 'Could not navigate to page' will be displayed for each topic in chm file). 

See also detailed instructions: http://www.drexplain.com/press/articles/error_accessing_and_displaying_chm_files_reasons_and_solutions/