clear;
close all;

fileMatrix = csvread('bow_newsletter.data');

vTri    = fileMatrix; % fill in similarities
n       = sqrt(numel(vTri)*2+1/4)+1/2;
b       = tril(ones(n),-1); % it would be triu, but the assignment is in the wrong order
b(b==1) = vTri;
Sim     = b;
Sim     = Sim'+Sim +diag(diag(ones(n)));

%%
[m,n] = size(Sim);
R = randsample(m, 100);

Knm = Sim*Sim(R,:)';
Kmm = Knm(R,:);

PinvKmm = pinv(Kmm); ny{1} = Knm; ny{2} = PinvKmm;
fKernel = @(X,Y)Knm(X,:)*PinvKmm*Knm(Y,:)';

Kny = fKernel(1:m,1:n);

figure('Name', 'Nyström');
imagesc(Kny);