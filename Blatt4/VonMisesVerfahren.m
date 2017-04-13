function [ eval, evec ] = VonMisesVerfahren( M, maxIter )
[n, ~] = size(M);
evec = ones(n, 1) / sqrt(n);
eps = 1e-8;

eval = 0;

for i=1:maxIter
    evec_new = M*evec;
    eval_new = (evec' * evec_new) / norm(evec)^2;
    
    shouldBreak = abs(eval_new-eval) < eps;
    
    eval = eval_new;
    evec = evec_new / norm(evec_new);
    
    if shouldBreak; break; end
end
end
