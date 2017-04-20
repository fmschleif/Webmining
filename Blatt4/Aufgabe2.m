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

figure('Name', 'DataImage');
imagesc(Sim);

symbolMap = {'x', 'o', 's', '*'};

figure('Name', 'PCA 3D');
colormap hsv
[e_vec, e_val] = eigs(Sim,3);
vis = e_vec'*Sim;
m = symbolMap(kron(ones(1,5),1:4));
for c=1:20
    r = (c-1)*100+1:c*100;
    scatter3(vis(1,r),vis(2,r),vis(3,r), [], ones(1,100)*c, m{c});
    hold on;
end
hold off;