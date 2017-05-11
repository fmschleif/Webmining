clear;
close all;
load('Sample.mat');

%% a)
figure('Name', '3D Plot before PCA');
scatter3(mDmapped(:,1), mDmapped(:,2), mDmapped(:,3), 50, L);

%% b)

m = mean(mDmapped, 1) ;
X = mDmapped - repmat(m, [size(mDmapped, 1) 1]);

C = cov(X);
[eig_vecs, eig_vals] = eigs(C,3);


%% c)
% 2 dimensional

%% d)

pcaReduced = X * eig_vecs;

figure('Name', '3D Plot after PCA');
scatter3(pcaReduced(:,1), pcaReduced(:,2), pcaReduced(:,3), 50, L);

figure('Name', '2D Plot after PCA');
scatter(pcaReduced(:,1), pcaReduced(:,2), 50, L);

