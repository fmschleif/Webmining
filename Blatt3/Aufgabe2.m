clear;
coder.extrinsic('ncd');

%% a/b)
fileContent = ReadFileLineByLine('newsletter_cleaned.data', 100);
fileContentAsAscii = cellfun(@int8, fileContent, 'UniformOutput', 0);
newsgroups = kron(1:10,ones(1,10));

%% c)
ncdMatrix = zeros(100, 100);
newsgroupCombMatrix = zeros(100, 100, 2);

for i=1:100
    for j=1:100
        ncdMatrix(i,j) = ncd(fileContentAsAscii{i}, fileContentAsAscii{j});
        newsgroupCombMatrix(i,j,1) = i;
        newsgroupCombMatrix(i,j,2) = j;
    end
end

%% d)
[X, Y] = meshgrid(1:100, 1:100);
scatter3(X(:), Y(:), ncdMatrix(:));