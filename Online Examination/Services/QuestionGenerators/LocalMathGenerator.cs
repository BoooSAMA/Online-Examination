using Online_Examination.Domain;

namespace Online_Examination.Services
{
    public static class LocalMathGenerator
    {
        private static readonly Random _random = new Random();

        public static List<Question> GenerateQuestions(string difficulty, int count = 5)
        {
            var questions = new List<Question>();

            for (int i = 0; i < count; i++)
            {
                Question question = difficulty.ToLower() switch
                {
                    "easy" => GenerateEasyQuestion(),
                    "medium" => GenerateMediumQuestion(),
                    "hard" => GenerateHardQuestion(),
                    "expert" => GenerateExpertQuestion(),
                    _ => GenerateEasyQuestion()
                };

                questions.Add(question);
            }

            return questions;
        }

        private static Question GenerateEasyQuestion()
        {
            int num1 = _random.Next(1, 50);
            int num2 = _random.Next(1, 50);
            int operation = _random.Next(0, 3); // 0: addition, 1: subtraction, 2: multiplication
            
            string questionText;
            int correctAnswer;

            switch (operation)
            {
                case 0: // Addition
                    correctAnswer = num1 + num2;
                    questionText = $"What is {num1} + {num2}?";
                    break;
                case 1: // Subtraction
                    // Ensure positive result
                    if (num1 < num2)
                    {
                        var temp = num1;
                        num1 = num2;
                        num2 = temp;
                    }
                    correctAnswer = num1 - num2;
                    questionText = $"What is {num1} - {num2}?";
                    break;
                case 2: // Multiplication
                    num1 = _random.Next(2, 15);
                    num2 = _random.Next(2, 15);
                    correctAnswer = num1 * num2;
                    questionText = $"What is {num1} ¡Á {num2}?";
                    break;
                default:
                    correctAnswer = num1 + num2;
                    questionText = $"What is {num1} + {num2}?";
                    break;
            }

            // Generate incorrect answers
            var incorrectAnswers = new List<int>
            {
                correctAnswer + _random.Next(1, 5),
                correctAnswer - _random.Next(1, 5),
                correctAnswer + _random.Next(5, 15)
            };

            return CreateShuffledQuestion(questionText, correctAnswer.ToString(), 
                incorrectAnswers.Select(x => x.ToString()).ToList());
        }

        private static Question GenerateMediumQuestion()
        {
            int questionType = _random.Next(0, 2); // 0: linear equation, 1: percentage

            if (questionType == 0)
            {
                // Simple linear equation: ax + b = c
                int a = _random.Next(2, 10);
                int x = _random.Next(1, 20);
                int b = _random.Next(1, 20);
                int c = a * x + b;

                string questionText = $"Solve for x: {a}x + {b} = {c}";
                int correctAnswer = x;

                var incorrectAnswers = new List<int>
                {
                    correctAnswer + _random.Next(1, 5),
                    correctAnswer - _random.Next(1, 5),
                    (c - b) / (a + 1) // Common mistake
                };

                return CreateShuffledQuestion(questionText, correctAnswer.ToString(), 
                    incorrectAnswers.Select(n => n.ToString()).ToList());
            }
            else
            {
                // Percentage problem
                int total = _random.Next(50, 200);
                int percentage = _random.Next(10, 50) * 5; // Multiples of 5
                int result = (total * percentage) / 100;

                string questionText = $"What is {percentage}% of {total}?";
                int correctAnswer = result;

                var incorrectAnswers = new List<int>
                {
                    correctAnswer + _random.Next(5, 15),
                    correctAnswer - _random.Next(5, 15),
                    total - correctAnswer
                };

                return CreateShuffledQuestion(questionText, correctAnswer.ToString(), 
                    incorrectAnswers.Select(n => n.ToString()).ToList());
            }
        }

        private static Question GenerateHardQuestion()
        {
            int questionType = _random.Next(0, 2); // 0: quadratic, 1: complex arithmetic

            if (questionType == 0)
            {
                // Quadratic equation: x^2 - (sum)x + (product) = 0
                // Where sum and product give us the roots
                int root1 = _random.Next(2, 8);
                int root2 = _random.Next(2, 8);
                int sum = root1 + root2;
                int product = root1 * root2;

                string questionText = $"Find the positive solution for x? - {sum}x + {product} = 0";
                int correctAnswer = Math.Max(root1, root2);

                var incorrectAnswers = new List<int>
                {
                    Math.Min(root1, root2),
                    correctAnswer + 1,
                    sum
                };

                return CreateShuffledQuestion(questionText, correctAnswer.ToString(), 
                    incorrectAnswers.Select(n => n.ToString()).ToList());
            }
            else
            {
                // Complex arithmetic with order of operations
                int a = _random.Next(2, 10);
                int b = _random.Next(2, 10);
                int c = _random.Next(2, 5);
                int correctAnswer = a * b + c * 2;

                string questionText = $"Calculate: ({a} ¡Á {b}) + ({c} ¡Á 2)";

                var incorrectAnswers = new List<int>
                {
                    (a + b) * c * 2, // Wrong order of operations
                    a * (b + c) * 2, // Wrong grouping
                    a * b + c        // Missing second multiplication
                };

                return CreateShuffledQuestion(questionText, correctAnswer.ToString(), 
                    incorrectAnswers.Select(n => n.ToString()).ToList());
            }
        }

        private static Question GenerateExpertQuestion()
        {
            // Basic Calculus - Power Rule Differentiation
            // f(x) = ax^n -> f'(x) = a*n*x^(n-1)
            
            int a = _random.Next(2, 10); // Coefficient (2-9)
            int n = _random.Next(2, 6);  // Exponent (2-5)

            // Build the question text
            string functionText = n == 2 ? $"{a}x?" : $"{a}x^{n}";
            string questionText = $"Find the derivative of f(x) = {functionText}";

            // Calculate correct answer using power rule
            int newCoefficient = a * n;
            int newExponent = n - 1;
            
            string correctAnswer;
            if (newExponent == 0)
            {
                correctAnswer = newCoefficient.ToString();
            }
            else if (newExponent == 1)
            {
                correctAnswer = $"{newCoefficient}x";
            }
            else if (newExponent == 2)
            {
                correctAnswer = $"{newCoefficient}x?";
            }
            else
            {
                correctAnswer = $"{newCoefficient}x^{newExponent}";
            }

            // Generate distractors based on common mistakes
            var incorrectAnswers = new List<string>();

            // Mistake 1: Forgot to multiply coefficient by exponent (only dropped power)
            string distractor1 = newExponent == 0 ? a.ToString() : 
                                newExponent == 1 ? $"{a}x" : 
                                newExponent == 2 ? $"{a}x?" :
                                $"{a}x^{newExponent}";
            incorrectAnswers.Add(distractor1);

            // Mistake 2: Forgot to drop power (only multiplied coefficient)
            string distractor2 = n == 2 ? $"{newCoefficient}x?" : $"{newCoefficient}x^{n}";
            incorrectAnswers.Add(distractor2);

            // Mistake 3: Integrated instead of differentiated (added 1 to power, divided by new power)
            int integratedExponent = n + 1;
            int integratedCoefficient = a; // Simplified, not dividing for clarity
            string distractor3 = integratedExponent == 2 ? $"{integratedCoefficient}x?" : 
                                $"{integratedCoefficient}x^{integratedExponent}";
            incorrectAnswers.Add(distractor3);

            System.Diagnostics.Debug.WriteLine($"[MathGenerator-Expert] Function: {functionText}");
            System.Diagnostics.Debug.WriteLine($"[MathGenerator-Expert] Derivative: {correctAnswer}");

            return CreateShuffledQuestion(questionText, correctAnswer, incorrectAnswers);
        }

        private static Question CreateShuffledQuestion(string questionText, string correctAnswer, List<string> incorrectAnswers)
        {
            // Ensure we have exactly 3 incorrect answers
            while (incorrectAnswers.Count < 3)
            {
                var newIncorrect = (int.Parse(correctAnswer) + _random.Next(-10, 10)).ToString();
                if (!incorrectAnswers.Contains(newIncorrect) && newIncorrect != correctAnswer)
                {
                    incorrectAnswers.Add(newIncorrect);
                }
            }

            // Take only first 3 incorrect answers
            incorrectAnswers = incorrectAnswers.Take(3).ToList();

            // Create list of all answers
            var allAnswers = new List<string> { correctAnswer };
            allAnswers.AddRange(incorrectAnswers);

            // Shuffle using Fisher-Yates algorithm
            for (int i = allAnswers.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                var temp = allAnswers[i];
                allAnswers[i] = allAnswers[j];
                allAnswers[j] = temp;
            }

            // Find correct answer position
            int correctIndex = allAnswers.IndexOf(correctAnswer);
            string correctAnswerLetter = ((char)('A' + correctIndex)).ToString();

            System.Diagnostics.Debug.WriteLine($"[MathGenerator] Question: {questionText}");
            System.Diagnostics.Debug.WriteLine($"[MathGenerator] Correct answer: {correctAnswer} at position {correctAnswerLetter}");

            return new Question
            {
                Text = questionText,
                OptionA = allAnswers[0],
                OptionB = allAnswers[1],
                OptionC = allAnswers[2],
                OptionD = allAnswers[3],
                CorrectAnswer = correctAnswerLetter
            };
        }
    }
}
