function [ fileContent ] = ReadFileLineByLine( filePath, lineCount )
if ~exist('arg1','var')
  lineCount=100;
end
fileContent = {lineCount};
index = 0;
fid = fopen(filePath);
tline = fgetl(fid);
while ischar(tline)
    index = index + 1;
    fileContent{index} = tline;
    tline = fgetl(fid);
end
fclose(fid);
end

