namespace QuantumLib {
    open Microsoft.Quantum.Intrinsic;
    open Microsoft.Quantum.Canon;
    open Microsoft.Quantum.Measurement;
    open Microsoft.Quantum.Math;
    open Microsoft.Quantum.Convert;
    open Microsoft.Quantum.Diagnostics;
    open Microsoft.Quantum.Arrays;
    open Microsoft.Quantum.Random;

    operation TeleportMulti (sourceQubits : Qubit[], receiverQubits : Qubit[], noiseLevel : Double) : Unit {
        let numBits = Length(sourceQubits);
        use aliceHelpers = Qubit[numBits];
        
        for idx in 0 .. numBits - 1 {
            H(aliceHelpers[idx]);
            CNOT(aliceHelpers[idx], receiverQubits[idx]);
            
            if (noiseLevel > 0.0) {
                ApplyNoise(noiseLevel, [aliceHelpers[idx], receiverQubits[idx]]);
            }
            
            CNOT(sourceQubits[idx], aliceHelpers[idx]);
            H(sourceQubits[idx]);
            
            let measSource = M(sourceQubits[idx]);
            let measHelper = M(aliceHelpers[idx]);
            
            if (measHelper == One) { X(receiverQubits[idx]); }
            if (measSource == One) { Z(receiverQubits[idx]); }
            
            Reset(aliceHelpers[idx]);
        }
    }

    operation PrepArbitraryStates (qubitsToPrep : Qubit[], anglesTheta : Double[], phasesPhi : Double[]) : Unit is Adj + Ctl {
        let count = Length(qubitsToPrep);
        for i in 0 .. count - 1 {
            Ry(anglesTheta[i], qubitsToPrep[i]);
            Rz(phasesPhi[i], qubitsToPrep[i]);
        }
    }

    operation EncodeBitFlipCode (dataQubit : Qubit, ancillaPair : Qubit[]) : Unit is Adj + Ctl {
        CNOT(dataQubit, ancillaPair[0]);
        CNOT(dataQubit, ancillaPair[1]);
    }

    operation CorrectBitFlip (encodedQubits : Qubit[]) : Unit {
        let syndrome1 = M(encodedQubits[1]) == M(encodedQubits[0]) ? Zero | One;
        let syndrome2 = M(encodedQubits[2]) == M(encodedQubits[0]) ? Zero | One;
        if (syndrome1 == One || syndrome2 == One) {
            X(encodedQubits[0]);
        }
    }

    operation RunTeleportTest (numQubits : Int, thetaVal : Double, phiVal : Double, noiseAmt : Double, useErrorCorrection : Bool, numRuns : Int) : Double {
        mutable goodTeleports = 0;
        let thetas = ConstantArray(numQubits, thetaVal);
        let phis = ConstantArray(numQubits, phiVal);
        
        for run in 1 .. numRuns {
            use (sources, receivers) = (Qubit[numQubits], Qubit[numQubits]);
            mutable ancillas : Qubit[] = [];
            if (useErrorCorrection && numQubits >= 1) {
                use tempAnc = Qubit[2];
                set ancillas = tempAnc;
                EncodeBitFlipCode(sources[0], ancillas);
            }
            
            PrepArbitraryStates(sources, thetas, phis);
            
            TeleportMulti(sources, receivers, noiseAmt);
            
            if (useErrorCorrection && numQubits >= 1) {
                CorrectBitFlip([receivers[0]] + ancillas);
            }
            
            Adjoint PrepArbitraryStates(receivers, thetas, phis);
            let check = MResetZ(receivers[0]);
            
            if (check == Zero) {
                set goodTeleports += 1;
            }
            
            ResetAll(sources + receivers);
            if (Length(ancillas) > 0) {
                ResetAll(ancillas);
            }
        }
        return IntAsDouble(goodTeleports) / IntAsDouble(numRuns);
    }

    operation ApplyNoise (errorProb : Double, affectedQubits : Qubit[]) : Unit {
        for q in affectedQubits {
            if (errorProb > 0.0 && RandomReal(0.0, 1.0) < errorProb) {
                let pauliChoice = RandomInt(0, 2);
                if (pauliChoice == 0) { X(q); }
                elif (pauliChoice == 1) { Y(q); }
                else { Z(q); }
            }
        }
    }

    @EntryPoint()
    operation QuickTest () : String {
        let rate = RunTeleportTest(1, PI() / 3.0, PI() / 4.0, 0.0, false, 100);
        return $"quick test: {rate * 100.0}% success";
    }
}
