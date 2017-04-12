%% 1)
v_1 = ones(1,7)*2
v_2 = (1:7)'
M = reshape(1:7^2, 7, 7)'

%% 2)
M(3,6)
M(3,:)
M(:,6)
M(1:2,4:6)
sum(diag(M))
triu(M)
M(M>33)
M(mod(M,2)==1)

%% 3)
v_1 * v_2
% => g�ltig: berechet das "Dot product"

%v_1' * v_2
% => ung�ltig: da zeilen von v_2 ungleich spalten von v_1'

M * v_1'
% => g�ltig: multipliziert v_1' mit M (transformation von v_1' in den
% durch M beschriebenen Raum)

M(3,:) + v_1
% => g�ltig: holt den 3. zeilenvector und adiert darauf v_1

M == M'
% => g�ltig: gibt eine logic matrix zur�ck welche f�r die symetrischen
% elemente true (1) ist