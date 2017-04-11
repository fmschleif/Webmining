clear;
close all;
load('iris_test_plain.mat');

%% a)
figure('Name', '2D Plots');
plotIdx = 0;
combs2d = combnk(1:size(irisInputs,1),2);
for i = 1:size(combs2d,1)
    plotIdx  = plotIdx + 1;
    subplot(2,3,plotIdx);
    scatter(irisInputs(combs2d(i,1),:), irisInputs(combs2d(i,2),:), 50, irisTargets');
    xlabel(sprintf('Dimension %d', combs2d(i,1)));
    ylabel(sprintf('Dimension %d', combs2d(i,2)));
    title(sprintf('Dimensions %d & %d', combs2d(i,1), combs2d(i,2)));
end

figure('Name', '3D Plots');
plotIdx = 0;
combs3d = combnk(1:size(irisInputs,1),3);
for i = 1:size(combs3d,1)
    plotIdx  = plotIdx + 1;
    subplot(2,2,plotIdx);
    scatter3(irisInputs(combs3d(i,1),:), irisInputs(combs3d(i,2),:), irisInputs(combs3d(i,3),:), 50, irisTargets');
    xlabel(sprintf('Dimension %d', combs3d(i,1)));
    ylabel(sprintf('Dimension %d', combs3d(i,2)));
    zlabel(sprintf('Dimension %d', combs3d(i,3)));
    title(sprintf('Dimensions %d & %d & %d', combs3d(i,1), combs3d(i,2), combs3d(i,3)));
end

%% b)
irisDataNormMean = irisInputs - repmat(mean(irisInputs, 2), 1, size(irisInputs,2));

irisDataNormStdDiv = irisDataNormMean ./ repmat(sqrt(mean(irisDataNormMean.^2, 2)), 1, size(irisInputs,2));

figure('Name', 'Normed (Dimension 3 & 4)');
subplot(1,3,1);
scatter(irisInputs(3,:), irisInputs(4,:), 50, irisTargets');
xlabel('Dimension 3');
ylabel('Dimension 4');
title('Original');
subplot(1,3,2);
scatter(irisDataNormMean(3,:), irisDataNormMean(4,:), 50, irisTargets');
xlabel('Dimension 3');
ylabel('Dimension 4');
title('Normalized Mean');
subplot(1,3,3);
scatter(irisDataNormStdDiv(3,:), irisDataNormStdDiv(4,:), 50, irisTargets');
xlabel('Dimension 3');
ylabel('Dimension 4');
title('Normalized StdDiv');
