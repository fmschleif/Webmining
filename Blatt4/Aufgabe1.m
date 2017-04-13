clear;
close all;
load('iris_test_plain.mat');

%% a)
irisDataNormMean = irisInputs - repmat(mean(irisInputs, 2), 1, size(irisInputs,2));

%% b)
irisDataCovariance = irisDataNormMean * irisDataNormMean' / size(irisInputs, 2);

%% c+d+e)
[eval, evec] = VonMisesVerfahren(irisDataCovariance, 1000)

%% f)
[evec_m, eval_m] = eig(irisDataCovariance)

%% g)
plot(diag(eval_m));