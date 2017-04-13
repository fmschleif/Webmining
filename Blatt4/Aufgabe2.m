clear;
close all;

fileMatrix = csvread('bow_newsletter.data');

%% a)
vTri    = fileMatrix; % fill in similarities
n       = sqrt(numel(vTri)*2+1/4)+1/2;
b       = tril(ones(n),-1); % it would be triu, but the assignment is in the wrong order
b(b==1) = vTri;
Sim     = b;
Sim     = Sim'+Sim +diag(diag(ones(n)));

%% c) (TODO: find a better way to visualise
[n,m] = size(Sim);
[X, Y] = meshgrid(1:n, 1:m);

surf(X,Y,Sim);
shading interp
